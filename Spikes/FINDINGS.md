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
| Struct `is [var v]` | 0.49 µs | 0 B |
| Union `is Some<int>(var v)` | 5.9 µs | 24 000 B |
| Closed record `is Some(var v)` | 6.0 µs | 24 000 B |

The union and the closed record are 12× slower than the struct on the bare construct-and-match loop; both pay one
heap object per Some. Between the two there is no measurable difference.

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

## Interim conclusion (day 1 of the time box)

* On the #1034 measurements the union is not better than the struct on any axis: same or worse size, one allocation per
  Some, a third invalid state, no list pattern, and the None naming clash. Its single advantage, compiler-checked
  exhaustiveness, also comes with the closed record, and with the analyzer (λ-rule) for the struct.
* The closed record is the shape that makes #862 traits cheap; it costs an allocation per Some and a nullable reference.
* If the structs stay (expected), #1036 (generated cross product) is the route for Traverse/Lift; #862 would need a
  boxing-free HKT encoding that C# does not have.
