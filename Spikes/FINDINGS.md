# Spike findings (work in progress)

Measured with SDK 11.0.100-rc.3.26522.148 on Windows 11, Release builds, net11.0. Every statement below is also an
assertion in `Funcky.Spike.Test`, unless marked as a benchmark number.

## What the C# 15 compiler in RC3 accepts

| Feature | Status |
|---|---|
| `union X(A, B);` nominal union, with body, `partial`, interfaces, constraints | works |
| type parameter as a case: `union Option<T>(T, None)` | works |
| union as a case of another union | works |
| identical case types `union D(int, int)` | CS0111 |
| `closed record` / `closed class` | works; implicitly abstract (`abstract` is CS9384), derived types must be in the same assembly, nested or not |
| exhaustive `switch` over a closed hierarchy | works, no catch-all needed, missing case is CS8509 |
| `closed enum` | CS0106, not implemented |

## What a union is

A `readonly struct` with a single `object Value` field, a constructor and implicit conversion per case,
`System.Runtime.CompilerServices.IUnion` and `[Union]`. Patterns on the union are patterns on `Value`.

Consequences for `Option<T>`:

* **Every struct case is boxed.** `Some<int>` costs one 24-byte object. Today's struct Option costs nothing. The closed
  record costs the same 24-byte object (its `Some` *is* the object).
* **`sizeof` is always 8.** Today's struct grows with its payload (`Option<Guid>` is 20 bytes, `Option<string>` 16).
  A register pair holds either; the union needs a dereference to get at the value.
* **`default` is a third state.** `default(UnionOption<T>)` has `Value == null`, matches neither case, and a switch over
  `Some`/`None` throws `SwitchExpressionException` at runtime. The compiler knows: it reports CS8655 ("does not handle
  some null inputs"), a warning. So the union gives Funcky's Option exactly the invalid-default problem Either and Result
  have today (`[NonDefaultable]`), and loses the "default is None" property that makes `Option` safe in fields and arrays.
* **Case types are separate types outside the union.** `None` is a type, so `UnionOption<T>.None` cannot also be the
  property it is today. The closed record has the same clash (nested `None` type vs. `None` property).
* **No list pattern.** `option is [var value]` is gone (a union has no Count/indexer); `option is Some<T>(var value)` and
  `option is Some<T>(5)` replace it.
* **The payload-as-case form** `union BareOption<T>(T, None)` removes the `Some` wrapper and the extra box for class
  payloads, but `BareOption<None>` is unconstructible (CS0457) and `Some(None)` collapses into `None`: the `Nullable<T>`
  flattening that Funcky's Option exists to avoid.
* **System.Text.Json in .NET 11 knows `IUnion`**: without a converter it writes the current case (`{"Value":5}` for
  `Some<int>`, `{}` for `None`). A converter with today's contract (transparent Some, `null` for None) is as easy as today.
* **Either** needs two wrapper case types even when `L == R`. **Result** can use `Exception` itself as the error case,
  which reads well (`result is Exception e`) and does not box the error.

## Shape 3: today's struct as a *custom union*

`closed` is rejected on structs, record structs and interfaces (CS0106). But the compiler recognizes a hand-written
struct as a union when it carries `[Union]` and implements `IUnion` (an `object Value` reporting the current case). RC3
then gives the struct everything a declared union gets: `option is T value` / `option is None` patterns, exhaustiveness
(CS8509 for a missing case), conversions from the case types through the constructors.

`CustomUnionOption<T>` keeps today's layout (`bool` + `T`) and adds the three members. Measured:

* **Nothing allocates** at rest, in construction, in `Match`, `Select`, `SelectMany` (unchanged from today).
* **`default` is `None`** because `Value` reports the cached `None` box for it: no third state, unlike the declared union.
* **`sizeof` is today's** (8 for `int`, 20 for `Guid`, 16 for `string`).
* **A pattern match goes through `object Value`.** With a concrete pattern (`is int value`) the JIT elides the box, 0 bytes.
  In a method generic over `T` (`is T value`) the shared code boxes once per match, 24 bytes for `int`. Reference payloads
  and `None` never allocate.
* **List pattern, `Option<T>.None` property, implicit conversion from `T`, JSON converter: unchanged.** The payload is the
  case, so `Some(None)` nests fine (`Value` returns the inner struct boxed, which is a different type from `None`).
* **Older frameworks**: an internal PolySharp-style polyfill of `UnionAttribute` and `IUnion` compiles on netstandard2.0
  and net8.0, and a net8.0 consumer of such a library matches exhaustively on the type (verified cross-assembly). A consumer
  with an older compiler just sees a struct with a `Value` property.

This is the shape that answers the #1034 question without giving anything up: the struct stays, the compiler-checked
exhaustiveness comes for free, and Either/Result can get it the same way (`Value` returns the left/right or ok/exception).

### Day 2: the non-boxing access pattern removes the last cost

The unions spec (`proposals/csharp-15.0/unions.md`) has a *non-boxing union access pattern*: a `bool HasValue` property
and one `bool TryGetValue(out Case value)` per case. The compiler prefers them over `Value` for type patterns. RC3
implements it (verified by decompiling the test assembly: `option is T value` in a generic method lowers to
`option.TryGetValue(out T item)`), and with it:

* **A match never allocates, in generic code too.** Day 1's "one box per match in generic code" is gone. Funcky's
  `Option<T>` already has `TryGetValue(out T)` (hidden behind λ0001); it needs `HasValue` and `TryGetValue(out None)`.
* Day 1's generic-match measurement (24 B) was partly the test helper's own `Convert.ToInt32(object)` box; the day 2
  tests use a constrained `GetHashCode()` call instead.
* Benchmark, construct-and-match in code generic over the payload: custom union 0.33 µs / 0 B, struct list pattern
  0.35 µs / 0 B.

**Either and Result** (`CustomUnionEither.cs`, `CustomUnionResult.cs`) get the same treatment on today's layout. The
cases are the payload types: `either is TLeft l` / `either is TRight r`, `result is T v` / `result is Exception e`,
no wrapper types. Sizes and allocations are today's; `default` reports a `null` Value, which turns Funcky's
`[NonDefaultable]` contract into a compiler warning (CS8655) at every switch that does not handle `null`, and the
`NotSupportedException` stays for `Match`.

**Union member provider** (`CustomUnionResult.cs`): the spec lets the union members live on a nested public interface
named `IUnionMembers` (static `Create(T)`/`Create(Exception)` factories define the cases, plus `Value`, `HasValue` and
the `TryGetValue` overloads). RC3 accepts it. Result:

* the constructors stay **private** (today's Either/Result have private constructors, Option's is internal);
* `Value` is an **explicit** interface implementation and does not appear on the struct (`result.Value` is CS1061);
  a *public* `Value` on the struct is otherwise mandatory (explicit `IUnion.Value` alone is CS9386);
* the non-boxing members must be declared on the provider too, or the compiler falls back to `Value` and boxes again.

This is the shape to ship: no new public surface except `HasValue`/`TryGetValue` (λ0001 territory) and the nested
interface.

**Union conversions** come with the cases: `Either<string, int> e = 5;`, `Result<int> r = exception;`,
`Option<int> o = default(None);` compile without any implicit operator. That is most of #809 for free (see below).

**Caveats found**

* Identical case types: `Either<int, int>` and `Result<Exception>` compile, but a type pattern resolves to ONE
  `TryGetValue` (RC3 takes the first case): `Right(2) is int` is false, and for `Result<Exception>` the error case is
  not `Exception`. `Match` is unaffected. (Without the non-boxing members both sides matched; neither is right.)
* `Option<object>`: the `object` case covers everything for the compiler (a `None` arm after `object` is CS8510), and
  `is object` lowers to the null check, so `Option<object>.None is object` is true at runtime.
* **System.Text.Json** (.NET 11) recognizes `IUnion` and builds a union contract from the public single-parameter
  constructors. With the provider shape there are none, and it **throws** ("must specify at least one union case")
  instead of falling back to the plain struct contract that writes `{}` for Either and Result today. With public
  constructors it writes the case itself (`5`, `{}` for None). Either way Funcky must ship converters for all three
  types in 4.0.0: #947 (4.1.0) moves up, at least the Either/Result half.
* The `None` case type is a new public struct next to the `Option<T>.None` property; inside `Option<T>` the type and
  the property coexist (type positions only see types).

## What a closed record is

Today's class hierarchy with compiler-checked exhaustiveness. One object per `Some`, a singleton `None`, `default` is
`null` and the nullable analysis, not a runtime exception, owns it. Record equality is structural. It is the only shape
on which the trait layer below is free.

## #862 traits

`K<F, A>` + static abstract trait members works unchanged since C# 11 (lang-ext). Three things matter for Funcky:

1. **Constraints**: an implementation of a static abstract member must repeat the interface's constraints exactly, so
   `where T : notnull` has to sit on the trait. Fine for Funcky, where every monad demands notnull; Lazy's
   `[DynamicallyAccessedMembers]` cannot be expressed at all.
2. **Boxing**: `K<F, A>` is an interface, so a struct Option (or the union struct) is boxed on every entry into a
   trait-generic algorithm and type-tested on exit. lang-ext does not pay this because all its types are classes. A closed
   record Option implements `K` directly, allocating exactly its result object. See the benchmark table below.
3. **What it buys**: `Generic.cs` has Traverse, Sequence, Lift (per arity, not per monad) and Kleisli composition written
   once; `TraitsTest.OneTraverseServesEveryContainerAndEveryEffect` covers today's hand-written
   `IEnumerable × Option`, `IEnumerable × Either`, `Option × Either` with one method. #1036 (generated cross product) is
   the alternative when the structs stay.

## Side finding: today's `Option.Select` allocates

`Option<T>.Select` is written as `Match(none: ..., some: item => selector(item))`; the lambda captures `selector`, so
every Select allocates a closure and a delegate, 80 bytes in Release, although the Option is a struct. A branch-based
Select (as all spike shapes use) is allocation-free. Independent of the spike's outcome.

## Benchmarks

BenchmarkDotNet 0.15.8, in-process short run, 1000 iterations per invocation. "Struct (branch Select)" is today's
struct with the allocation-free Select/SelectMany from the side finding above, which is the fair baseline.

**Construct → Select → SelectMany → Match**

| Shape | Mean | Allocated |
|---|---|---|
| Struct today | 44.5 µs | 80 000 B |
| Struct (branch Select) | 7.3 µs | 0 B |
| Union (`Some<T>`/`None` cases) | 24.7 µs | 72 000 B |
| Union (`T` as case) | 19.8 µs | 48 000 B |
| Closed record | 20.2 µs | 64 000 B |

**Construct → pattern match** (no delegates)

| Shape | Mean | Allocated |
|---|---|---|
| Struct `is [var v]` | 0.33 µs | 0 B |
| Struct `is [var v]` in generic code | 0.35 µs | 0 B |
| Custom union (shape 3) `is int value` | 0.32 µs | 0 B |
| Custom union (shape 3) `is string value` | 0.32 µs | 0 B |
| Custom union (shape 3) `is T value` in generic code (day 2, TryGetValue) | 0.33 µs | 0 B |
| Union `is Some<int>(var v)` | 4.7 µs | 24 000 B |
| Closed record `is Some(var v)` | 4.5 µs | 24 000 B |

The declared union and the closed record are 14× slower than the struct on the bare construct-and-match loop; both pay
one heap object per Some. Between the two there is no measurable difference. The custom union on today's layout is
indistinguishable from the struct's list pattern.

**#862: 1000 Map steps through the trait**

| Variant | Mean | Allocated |
|---|---|---|
| Struct, direct Select (today) | 25.4 µs | 80 000 B |
| Struct, direct branch Select | 3.4 µs | 0 B |
| Struct through `K` (boxed) | 25.6 µs | 48 024 B |
| Union through `K` (boxed) | 19.4 µs | 48 048 B |
| Closed record through `K` wrapper | 18.7 µs | 48 048 B |
| Closed record implementing `K` directly | 9.8 µs | 24 024 B |

**Traverse over 1000 items with an Option effect**

| Variant | Mean | Allocated |
|---|---|---|
| Hand-written `IEnumerable.Traverse` (today) | 22.4 µs | 12.2 KB |
| Trait Traverse, struct Option boxed | 88.5 µs | 313 KB |
| Trait Traverse, closed record direct | 17.8 µs | 55 KB |

Reading: traits over the struct cost 7× the time and 25× the memory of the hand-written code, which is the price of
boxing into `K` at every step. Traits over a class-shaped Option are competitive with today's hand-written code (the
remaining allocations are the `List<B>` accumulator and the Some objects), but the class shape itself costs a heap object
per value everywhere else.

## The dependents against the custom-union shape (day 2)

| Issue | What the shape changes |
|---|---|
| #809 simpler constructors | Union conversions do it: `Either<string, int> e = 5` / `= "x"`, `Result<int> r = exception`, `Option<int> o = None` (with a `None` value in a prelude). `Either<T, T>` stays on `Left`/`Right` (ambiguous conversion is a compile error). Open: a `None` singleton value and where it lives. |
| #760 collection expressions | Independent. The struct keeps `Count`/indexer, so `[var value]` stays; the case patterns `is T value` / `is None` make the list pattern redundant rather than wrong. A collection builder is additive. |
| #868 UpCast/DownCast extensions | Partly subsumed: `option is Derived d` applies the pattern to the case, so a DownCast in a condition is a pattern. The fluent `.DownCast<T>()` is still wanted for chains; independent of the shape. |
| #864 Task as a monad | Independent. Task is a class with its own shape; this is #1036 territory (generated Task × Option/Result rows). |
| #802 Validation | New type, same recipe: custom union with cases `T` and the error collection, provider interface, converters. |

## Breaking surface of the custom-union shape (for #1033)

* `[Union]` + `IUnion` on `Option<T>`, `Either<L, R>`, `Result<T>`; nested public `IUnionMembers` interface per type;
  new public `HasValue` and `TryGetValue` overloads on all three (λ0001 extends to them). Polyfilled `UnionAttribute`/
  `IUnion` (internal) on net8.0–net10.0 and netstandard, invisible to consumers on older compilers.
* New public struct `None` in `Funcky.Monads` (the Option case type).
* `default(Either)`/`default(Result)` in a switch: new CS8655 warning (was runtime-only).
* System.Text.Json: Either and Result serialize through new converters (today `{}`); Option's converter is unchanged.
* Not breaking: list pattern, `Option<T>.None` property, implicit conversion from `T`, `Match`/`Switch`, sizes,
  allocation profile, equality.

## Interim conclusion (end of day 2)

* On the #1034 measurements the declared union is not better than the struct on any axis: same or worse size, one
  allocation per Some, a third invalid state, no list pattern, and the None naming clash.
* Its single advantage, compiler-checked exhaustiveness, is available to today's struct as a custom union (shape 3)
  at no cost at all once the non-boxing access members are in place. That is the recommended direction for 4.0.0:
  keep the structs, add `[Union]`/`IUnion`, a provider interface with `Value`/`HasValue`/`TryGetValue` to Option,
  Either and Result (polyfilled below net11.0), ship JSON converters for Either and Result.
* The closed record is the shape that makes #862 traits cheap; it costs an allocation per Some and a nullable reference,
  which the custom-union struct does not justify giving up.
* With the structs staying, #1036 (generated cross product) is the route for Traverse/Lift; #862 would need a
  boxing-free HKT encoding that C# does not have.
