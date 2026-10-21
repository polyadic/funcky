# Changelog
All notable changes to this project will be documented in this file.
Funcky adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Funcky 3.8.0

* The extension methods of Funcky, Funcky.Async and Funcky.Xunit are now C# 14 extension members, declared in
  `extension(...)` blocks. The compiled methods keep their names and `this` parameters, so the change is source and
  binary compatible. Only `Interleave`, `And`, `Or` and `SplitLazy`, which take a `params` array, and the
  source-generated `…OrNone` and `Apply` methods stay classic extension methods for now: the C# compiler of the .NET
  SDK 10.0.1xx does not apply the expanded form to `params` parameters of extension members in referenced assemblies
  (fixed in the 10.0.2xx SDK).

## Funcky 3.7.0 | Funcky.Analyzers 2.0.0

The analyzer release: Funcky itself gains built-in correctness rules, `Funcky.Analyzers` gains eight style rules and a
code fix for a compiler warning, and the two packages settle which kind of rule lives where. The analyzer package becomes
2.0.0 because it drops a supported Roslyn version, which is a breaking change under semantic versioning.

### Analyzers
The analyzer rules now follow one written rule for which package a rule lives in: a rule is **built into the `Funcky`
package** when the flagged code is outside the supported surface of the library (the code is wrong, not merely
unidiomatic), and it lives in the opt-in **`Funcky.Analyzers`** package when the code works but Funcky offers a more
idiomatic form. The built-in rules report errors and carry the category `Funcky.Correctness`, the opt-in rules report
warnings (an info for pure performance hints), come with a code fix and carry the category `Funcky.Style`. See the
[analyzer rules](https://polyadic.github.io/funcky/analyzer-rules/analyzer-rules.html) page.

* Breaking (Funcky.Analyzers): the package no longer ships an asset for Roslyn 4.0; the analyzers require Roslyn 4.4
  (Visual Studio 2022 17.4, .NET SDK 7.0.100) or newer. The Roslyn 4.0 asset was never tested and targeted a Visual
  Studio version that is out of support.
* Breaking: `λ1009` "Do not use `default` to instantiate this type" and `λ1010` "An option has either zero or one
  elements" moved from `Funcky.Analyzers` into the built-in analyzers, so every consumer of `Funcky` now gets them
  without installing the analyzer package. The rule IDs stay the same, existing `.editorconfig` entries keep working.
  Funcky.Analyzers 2.0.0 no longer contains the two rules; an older Funcky.Analyzers next to Funcky 3.7.0 reports
  `λ1009` twice until it is updated.
* Breaking: the diagnostic category of every rule changed from `Funcky` to `Funcky.Correctness` (built-in) or
  `Funcky.Style` (opt-in). A `dotnet_analyzer_diagnostic.category-Funcky.severity` entry in an `.editorconfig` needs to
  name the new categories; per-rule `dotnet_diagnostic.λ….severity` entries are unaffected. The split lets you tone down
  all style rules with one line without touching the correctness rules.
* New: `FunckyDisableBuiltinAnalyzers` MSBuild property. Setting it to `true` removes the built-in analyzers from the
  build, the escape hatch for environments where they cannot run. `ExcludeAssets="analyzers"` on the package reference
  has the same effect.
* Fix: the built-in analyzers are shipped once per supported Roslyn version (`roslyn3.7` and `roslyn4.4`) and the
  package's `buildTransitive` targets keep only the `roslyn3.7` assets on SDKs without analyzer versioning, the same
  arrangement System.Text.Json uses. `λ1010` needs the Roslyn 4.4 list pattern API and is only active there, which
  matches the compilers that can use list patterns on `Option<T>` at all.
* New: the attributes behind `λ1003` (`[UseWithArgumentNames]`) and `λ1009` (`[NonDefaultable]`) can be used on your own
  members. The analyzers now match the attributes by name, so a copy in any assembly counts, and `Funcky.Analyzers`
  generates that copy when the project sets `<FunckyCodeAnalysisAttributes>UseWithArgumentNames;NonDefaultable</FunckyCodeAnalysisAttributes>`.
* Fixed: `λ1003` and `λ1009` went silent in a project referencing two assemblies that both define the attribute.
* Fix: the built-in analyzer λ0001 applied its allowance rules inconsistently. `TryGetValue` is now allowed anywhere inside
  a `catch` filter clause, not only as its outermost expression, so `catch when (option.TryGetValue(out var h) && h.CanHandle(e))`
  compiles. Conversely, a call inside a lambda or local function that is merely used by a loop condition, a catch filter or
  an `if` condition in an iterator, such as `while (items.Any(i => i.Option.TryGetValue(out _)))`, is now reported; it
  used to slip through because the check walked all ancestors. The exemption for `.cshtml` files finally applies: it
  recognizes Razor pages and views deriving from `RazorPageBase`, where it used to require a Blazor `ComponentBase`.
  The documentation of λ0001 now lists every allowed use case, including `if` statements that yield.
* New: built-in rule `λ0004` reports a bare `default` literal in a branch of a conditional, `switch` or `??` expression
  that is implicitly converted to `Option<T>`, `Result<T>` or `Either<L, R>`. The literal takes the type of the sibling
  branch, so `condition ? 42 : default` is `Some(0)` and never `None`; for value types and tuples the compiler is silent
  about it. The code fix replaces the literal with `Option<T>.None`.
* New: built-in rule `λ0005` reports a `null` literal that is converted to `Option<T>`, `Result<T>` or `Either<L, R>`
  or passed to one of their factory methods (`Option.Some`, `Result.Ok`, `Either<L, R>.Left`, …). The monads reject
  `null` with an `ArgumentNullException`, so `Option<string> option = null;` and even `option == null` throw at run time;
  with nullable reference types disabled the compiler is silent about it. The code fix replaces the literal with
  `Option<T>.None`.
* New: rule `λ1011` reports a `Match` whose failure branch returns the monad's own empty or error state and whose success
  branch returns a plain value, e.g. `option.Match(none: Option<string>.None, some: x => x.ToString())`: that is `Select`.
  The code fix rewrites the call for `Option<T>`, `Result<T>` and `Either<L, R>`.
* New: rule `λ1012` reports `FirstOrDefault`, `LastOrDefault`, `SingleOrDefault` and `ElementAtOrDefault` whose result is
  immediately tested for `null` (`?? fallback`, `?.`, `is null`, `is {} x`, `== null`, …) and suggests the `…OrNone`
  variant, which makes the missing element a `None` instead of a `null`. The code fix rewrites `xs.FirstOrDefault() ?? fallback`
  to `xs.FirstOrNone().GetOrElse(fallback)`, passes a fallback that does work as a function and adds the
  `using Funcky.Extensions;` when needed.
* New: rule `λ1013` reports `Sequence.Return(x)` in a position with a target type of `IEnumerable<T>`,
  `IReadOnlyCollection<T>` or `IReadOnlyList<T>` and suggests the collection expression `[x]`, which the code fix
  produces. It needs C# 12 and only fires where the collection expression binds the same way and keeps the element type;
  `var x = Sequence.Return(1)` and the receiver of a LINQ chain are left alone. The rule reports an info.
* New: rule `λ1014` reports a `throw` expression inside a fallback passed to the eager overload of `GetOrElse`, `OrElse`
  or `Match` on `Option<T>`, `Result<T>` and `Either<L, R>`, e.g. `option.GetOrElse(fallback ?? throw …)`. The argument
  is evaluated before the call, so it throws even when there is a value. The code fix passes a function instead.
* New: rule `λ1015` reports a fallback passed to the eager overload of `GetOrElse`, `OrElse` or `Match` on `Option<T>`,
  `Result<T>` and `Either<L, R>` that does work (a method call, an object creation, an interpolated string, an `await`),
  e.g. `option.GetOrElse(LoadDefault())`: the work happens even when there is a value. The code fix passes a function
  instead. As a performance hint rather than a correctness issue the rule reports an info, the first rule in the package
  to do so.
* New: rule `λ1016` reports a lambda passed to the lazy overload of `GetOrElse`, `OrElse` or `Match` on `Option<T>`,
  `Result<T>` and `Either<L, R>` that only returns a literal, a constant, a variable or a member, e.g.
  `option.GetOrElse(() => 0f)`: there is no work to defer, so the eager overload taking the value says the same with
  less. The code fix replaces the lambda with its body.
* New: rule `λ1017` reports a lambda that spells out one of the named functions in `Funcky.Functional`: `x => x` is
  `Identity`, `_ => true` is `True`, `_ => false` is `False` and `() => { }` is `NoOperation`. It only reports where the
  method group binds the same way as the lambda, so an ambiguous `Where(True)` is left alone. The code fix replaces the
  lambda and adds `using static Funcky.Functional;` when needed. Before C# 11 the rule reports an info, because the
  compiler only caches method group delegates from C# 11 on.
* New: rule `λ1018` reports `SelectMany(x => x)` and `SelectMany(Identity)` on a nested `Option`, `Result`, `Either` or
  `IEnumerable` and suggests `Flatten()`, which says what happens without a selector to read. The code fix replaces the call.
* New: a code fix for the compiler warning CS8714 ("Nullability of type argument doesn't match 'notnull' constraint") on
  `Option<T>.Select`. A selector that can return `null` is rewritten to `SelectMany` with `Option.FromNullable`, so
  `option.Select(user => user.MiddleName)` becomes `option.SelectMany(user => Option.FromNullable(user.MiddleName))`.
  The fix handles expression-bodied and block-bodied lambdas and supports fix-all; `Either` and `Result` get no fix
  because they need a failure value.

## Funcky 3.6.1 | Funcky.Async 1.5.0 | Funcky.Xunit 2.2.0 | Funcky.Xunit.v3 1.0.0 | Funcky.Analyzers 1.4.2

* New: `AggregateOrNone` and `AggregateOrNoneAsync`, the `…OrNone` variant of the seedless `Aggregate`, which throws for an
  empty sequence. The async variant also takes an accumulator receiving the cancellation token.
* New: `And`, `Or` and `Not` extension methods on `Func<T, bool>` in `Funcky.Extensions`, the extension form of `All`,
  `Any` and `Not`. The type argument is inferred from the receiver, so `isActive.And(isAdmin)` needs no `<User>`.
* New: `Then` on `Func<…>`, the forward form of `Compose`: `f.Then(g)` is `x => g(f(x))`, so a pipeline reads in the
  order it runs. Overloads mirror `Compose`, including a nullary function and an `Action` as the last step.
* New: `FunctionalAssert.Error<TException, TValidResult>(result)` asserts that a `Result` is an error of exactly the given
  exception type and returns it typed, the analogue of `Assert.Throws`.
* Fix: the xUnit v3 `Option` serializer no longer throws an `InvalidOperationException("TODO")` when asked whether a
  `null` value is serializable; it answers `false` and reports a failure reason like the interface asks for.
* Deprecation: the obsolete `ImmutableListExtensions` (superseded by `ListExtensions` in 3.0) now announces its removal in
  Funcky 4.0.0 in the obsolete message.
* Documentation: `SplitLines` states that a trailing new line adds no empty line, that an empty text yields nothing and
  that only CR, LF and CRLF are recognized.
* New: `Unit` overrides `ToString` (prints `Unit` instead of the type name) and implements the non-generic `IComparable`.
* Fix: `Flatten` (Option, Either, Lazy, IEnumerable), `Option.ToEither`, `Option.ToNullable`, `Either.SelectLeft`,
  `Option.ToAsyncEnumerable` and the string `Chunk` and `SlidingWindow` are marked `[Pure]` like their siblings.
* New: `GetNonEnumeratedCountOrNone` is available on every target framework; it was limited to .NET 6 and later although
  the polyfill for the underlying check was already shipped. On older frameworks only collections with a `Count` are
  recognized.
* Fix: `Option<T>.Where` and `Option.FromBoolean(bool, T)` no longer allocate a closure per call.
* Fix: `SlidingWindow` and `SlidingWindowAsync` kept the window in an `ImmutableQueue` and built an `ImmutableList` for
  every window. They now use a ring buffer and copy each window into a read-only array, which is O(width) per window
  with a single allocation instead of a tree per window.
* Fix: `WhereSelect` and its async variants no longer allocate an enumerable per element; they are plain iterators now and
  validate their arguments when called like the other iterator-based extensions.
* Fix: `AnyOrElse` with a fallback function invoked the function when `AnyOrElse` was called if the source was an empty
  collection, but when the result was enumerated otherwise. The function is now always invoked on enumeration.
* Fix: every analyzer rule now carries a help link to its documentation page (`https://polyadic.github.io/funcky/analyzer-rules/<id>.html`);
  previously only λ0001 had one.
* Fix: the `FunctionalAssert` code fix (λ1101) was exported under the name of the argument-name code fix, which broke
  fix-all grouping in the IDE.
* Fix: the `NonDefaultable` and `SyntaxSupportOnly` marker attributes are removed by the trimmer like `UseWithArgumentNames`
  already was; all three are compile-time-only.
* Packaging: the Funcky.Analyzers package declared its version in a property the SDK does not read, so every package
  built since February 2025 outside a release was versioned 1.0.0. It uses `VersionPrefix` like the other packages.
* Packaging: the Funcky and Funcky.Analyzers packages no longer ship the `tools/install.ps1` and `uninstall.ps1` hooks,
  which `PackageReference` never runs.
* Fix: `AverageOrNone` and `AverageOrNoneAsync` on `long` (and `Option<long>`) summed in `double` and lost precision for
  large values; they now sum exactly in a checked `long` like `Enumerable.Average`, so the result matches the BCL and a sum
  that overflows `long` throws `OverflowException` instead of returning an inexact average. The `float` overloads sum in
  `double` like the BCL. The element count is a `long`, so sequences with more than two billion elements are averaged
  correctly. Internally the three per-type aggregator classes became one allocation-free generic struct.
* Fix: `Interleave` and `Merge` (sync and async) acquired the enumerators of all sources before entering the `try` that
  disposes them, so if the `GetEnumerator` of one source threw, the enumerators of the sources before it were never
  disposed. They are now acquired inside the `try` and the ones acquired so far are disposed.
* Fix: `Sequence.RepeatRange` and `AsyncSequence.RepeatRange` reject a negative `count` with an
  `ArgumentOutOfRangeException` when they are called, like `RepeatMaterialized` and `Enumerable.Repeat`. Previously a
  negative count silently yielded the source once. The XML docs of all three methods described the result as infinite;
  it contains the source exactly `count` times.
* Documentation: the Funcky.Xunit readme claimed that `AggressiveInlining` removes the `FunctionalAssert` frame from the
  stack trace of a failed assertion on frameworks without `[StackTraceHidden]`. The JIT never inlines a method with an
  exception handler, so the attribute was dead; it is removed and the readme now states that on .NET 6 and later the
  trace starts at the test method, and on older frameworks with a single `FunctionalAssert` frame above it. Both
  behaviours are now covered by tests, the latter on .NET Framework 4.8.
* Build: the public API of every package is now tracked per target framework. The `PublicAPI.Shipped.txt` and
  `PublicAPI.Unshipped.txt` files moved to `PublicAPI/<target framework>/` and the public API analyzer runs for every
  target framework instead of only the newest one. The old single file listed the async APIs for frameworks that do
  not have them and did not know about APIs that only exist on older frameworks, so a breaking change on those was
  not detected.
* Fix: the `OrNoneFromTryPattern` source generator handles try methods it previously generated broken code for or
  crashed on: `in`, `ref` and `ref readonly` parameters are forwarded with their modifiers instead of being dropped from
  the signature, non-null default values (numbers, strings, chars, booleans, enums and `default` structs) are copied
  instead of throwing at generation time, overloads that do not follow the try pattern (no or several `out`
  parameters, non-`bool` return, instance methods) are skipped instead of crashing, and two attributed source files
  with the same file name no longer collide on the generated hint name. Generated methods now carry a summary, so
  the `Parse…OrNone` family shows documentation in IntelliSense.
* New: the generic `ParseOrNone<T>` (string, `ReadOnlySpan<char>` and UTF-8 `ReadOnlySpan<byte>`) and
  `ParseNumberOrNone<T>` (string and `ReadOnlySpan<char>`) gain overloads without the `IFormatProvider`, which use the
  current culture like the generated `Parse…OrNone` methods, and `ParseNumberOrNone<T>` gains overloads without the
  `NumberStyles`, which use the number type's default style.
* Fix: `ParseNumberOrNone<T>` on strings accepts a `null` candidate and returns `None`, like `ParseOrNone<T>` and the
  generated parse methods. All generic parse methods are now marked `[Pure]` and documented.
* Fix: `Sequence.Return(params T[])` returned the array itself typed as `IReadOnlyList<T>`, so the result could be cast
  back to the array and modified. It now returns a read-only view over the array (`Array.AsReadOnly`), which still
  does not copy. The XML docs explain that an array passed in is viewed, not copied, and that passing an array binds
  to the `params` overload rather than producing a one-element sequence.
* Breaking: the generic `ParseEnumOrNone<TEnum>` overloads are constrained to `struct, Enum` instead of `struct`,
  so `"1".ParseEnumOrNone<int>()` is a compile error instead of an `ArgumentException` at runtime.
* Fix: the `string` overloads of `ParseEnumOrNone` accept a `null` candidate and return `None`, like every other
  `Parse…OrNone` method on strings.
* Fix: on frameworks without `ExceptionDispatchInfo.SetCurrentStackTrace` (.NET Framework, .NET Standard,
  .NET Core 3.1), `Result<T>.Error` captures the stack trace by reflecting on private runtime members in a static
  initializer. On a runtime that names them differently or cannot compile expression trees, the first `Error` call
  failed with a `TypeInitializationException`. The lookup is now defensive: if it fails, the stack trace is simply
  not captured and `Error` works as usual.
* Fix: `OptionEqualityComparer<T>.Default` and `OptionComparer<T>.Default` created a new comparer instance on every
  read, and every `Equals`, `GetHashCode`, `==` and `CompareTo` on an `Option<T>` went through them and through
  several delegate allocations. Both defaults are now cached per item type like `EqualityComparer<T>.Default`, and
  equality, hashing and comparison of options no longer allocate.
* Fix: `GetHashCode` and `ToString` of `Either<L, R>` and `GetHashCode` of `EitherOrBoth<L, R>` threw
  `NotSupportedException` for a value constructed with `default`, so such a value broke `HashSet`, `Dictionary`,
  `Distinct`, string interpolation and logging. Both members are now total like `Equals` already was: a defaulted
  value hashes to 0 and prints as `Uninitialized`. `Match`, `Switch` and everything built on them still throw.
* New: `EitherOrBoth<L, R>` overrides `ToString` (`Left(…)`, `Right(…)`, `Both(…, …)`).
* Fix: a `Result<T>` constructed with `default` behaved like `Ok` with a `null` value, bypassing the null check of
  `Result.Ok`. `Match`, `Switch` and every operation built on them now throw `NotSupportedException` for such a
  value, like `Either<L, R>` already did. `Equals`, `GetHashCode` and `ToString` stay total. The analyzer λ1009
  still reports `default(Result<T>)` at compile time; see polyadic/funcky#934 for making it a built-in analyzer.
* Fix: `Shuffle` and `ShuffleAsync` without a `Random` argument used `new Random()` per call, which on .NET Framework
  is seeded from the clock, so two shuffles within the same tick returned the same order. They now draw from
  `Random.Shared` on .NET 6 and later and from a shared, per-thread generator on older frameworks.
* Fix: on frameworks before .NET 8, `Shuffle` built its result by appending to an `ImmutableList`, which is
  O(n log n) and allocates per element. It now shuffles an array in place like the .NET 8 implementation, so the
  result is O(n) and identical for a given seed on every target framework. Both overloads also drop the `[Pure]`
  attribute, since they advance the state of the random number generator.
* Documentation: the XML docs of `Shuffle` and `ShuffleAsync` claimed deferred execution; both are eager.
* Fix: `Memoize` and `CycleRange`/`RepeatRange` for `IEnumerable` no longer acquire the source's enumerator when the
  buffer is created, but when the first element is requested, so a source that opens a resource or throws from
  `GetEnumerator` does so lazily and a buffer that is disposed unused never touches the source. The source enumerator
  is disposed as soon as it is exhausted instead of when the buffer is disposed, and `Memoize` can now be enumerated by
  several threads concurrently. The `IAsyncEnumerable` cycle buffer no longer calls `MoveNextAsync` on an exhausted
  source, which rewound a .NET 10 compiler-generated iterator and repeated the source instead of the buffer.
* Fix: the read-only view that `Materialize` returns for an `IList<T>` or `ICollection<T>` source can no longer be
  used to modify the source through a cast to the mutable interface: `IsReadOnly` is `true` and `Add`, `Clear`,
  `Remove`, `Insert`, `RemoveAt` and the indexer setter throw `NotSupportedException`, like `ReadOnlyCollection<T>`.
  The view still reflects later changes to the source; `Materialize` evaluates a lazy sequence once, it does not
  take a snapshot.
* Fix: the iterator-based `IAsyncEnumerable` extensions (`AdjacentGroupBy` and its `Await` variants, `AnyOrElse`,
  `Chunk`, `Inspect`, `InspectEmpty`, `Interleave`, `Memoize`, `Merge`, `Pairwise`, `PowerSet`, `InclusiveScan`,
  `ExclusiveScan` and their `Await` variants, `SlidingWindow`, `Split`, `Transpose`, `WithFirst`, `WithLast`,
  `WithPrevious`, `ZipLongest`) and `AsyncSequence.Successors` now validate their arguments when they are called,
  like LINQ: a `null` source, selector, comparer or fallback throws an `ArgumentNullException` naming the
  parameter immediately, instead of a `NullReferenceException` when the result is first enumerated.
* Fix: `WithFirst`, `WithLast` and `ZipLongest` for `IAsyncEnumerable` now pass the cancellation token
  given to `GetAsyncEnumerator` (e.g. via `WithCancellation`) on to the source sequence.
* Fix: the iterator-based `IEnumerable` extensions (`AdjacentGroupBy`, `AnyOrElse`, `Chunk`, `Inspect`,
  `InspectEmpty`, `Interleave`, `Memoize`, `Merge`, `Pairwise`, `PowerSet`, `InclusiveScan`, `ExclusiveScan`,
  `SlidingWindow`, `Split`, `Transpose`, `WithFirst`, `WithLast`, `WithPrevious`, `ZipLongest`) and
  `Sequence.Successors` now validate their arguments when they are called, like LINQ: a `null` source,
  selector, comparer or fallback throws an `ArgumentNullException` naming the parameter immediately,
  instead of a `NullReferenceException` when the result is first enumerated.
* Breaking: `Split` on `IEnumerable<T>` and `IAsyncEnumerable<T>` now behaves exactly like `string.Split`:
  a separator at the end of the source produces a trailing empty part, and an empty source produces a single
  empty part. Previously both cases swallowed the empty part, so `[1, 0].Split(0)` gave `[[1]]` while
  `[0, 1].Split(0)` gave `[[], [1]]`, and the source could not be reconstructed from the parts. The result
  now always has exactly one part more than there are separators.
* Fix: `MinByOrNone` and `MaxByOrNone` now treat `null` keys like `Enumerable.MinBy`/`Enumerable.MaxBy`:
  elements whose key is `null` are never chosen as the minimum or maximum, and if all keys are `null`
  the first element is returned. Previously `MinByOrNone` returned the first element with a `null` key,
  because the default comparer orders `null` before everything else. Both methods also evaluate the key
  selector only once per element instead of twice.
* Fix: the retry policies (`ConstantDelayPolicy`, `NoDelayRetryPolicy`, `LinearBackOffRetryPolicy`,
  `ExponentialBackOffRetryPolicy`) now reject a negative `maxRetries` with an `ArgumentOutOfRangeException`
  when they are constructed, instead of failing later inside `Retry`.
* Fix: `ConstantDelayPolicy`, `LinearBackOffRetryPolicy` and `ExponentialBackOffRetryPolicy` now reject a
  negative delay with an `ArgumentOutOfRangeException` when they are constructed, instead of failing
  later from `Thread.Sleep` or `Task.Delay`.
* Fix: the NuGet packages produced by CI (nightly feed and release artifacts) were built in the Debug
  configuration. They are now built in Release.
* Fix: cancellation gaps in the async APIs (`Funcky.Async`, and `Funcky` itself on .NET 10):
  * `AsyncSequence.Successors` now honours the cancellation token passed to `GetAsyncEnumerator`
    (e.g. via `WithCancellation`) and stops before computing the next element.
  * `Memoize` and `CycleRange`/`RepeatRange` for `IAsyncEnumerable` no longer acquire the source's enumerator
    when the buffer is created, but when the first element is requested, and they honour each consumer's
    cancellation token. The memoized buffer can now be enumerated by several consumers concurrently.
  * `AnyOrElse` for `IAsyncEnumerable` now awaits with `ConfigureAwait(false)` like the other operators.
  * The exception-based `RetryAsync`/`RetryAwaitAsync` no longer retry once the cancellation token is cancelled,
    even if `shouldRetry` matches the exception, and check the token before the first attempt.
* New: `RetryAsync` and `RetryAwaitAsync` overloads whose producer receives the `CancellationToken`,
  so an attempt in progress can be aborted.
* Fix: the `Retry` and `RetryAsync` overloads without a retry policy were implemented recursively, so a
  producer that returned `None` often enough crashed the process with a `StackOverflowException`.
  They are now plain loops.
* Fix: the `Option`-based `Retry` and `RetryAsync` overloads numbered the retries from 0 while the
  exception-based overloads numbered them from 1, so the same `IRetryPolicy` produced different delays
  depending on the overload. With a `LinearBackOffRetryPolicy` the `Option`-based overloads retried
  immediately the first time. `IRetryPolicy.Delay(retryCount)` is now documented as one-based
  (`Delay(1)` is the wait before the first retry) and all overloads follow that contract.
* Fix: `ExponentialBackOffRetryPolicy` now waits `firstDelay` before the first retry, like
  `LinearBackOffRetryPolicy`. Previously the first delay was `firstDelay * 1.5` with the
  exception-based `Retry` overloads.
* Fix: `Transpose` enumerated the outer sequence three times, two of them eagerly when the method was
  called. It now enumerates the outer sequence exactly once, lazily, when the first column is requested. (#895)
* Breaking: `Transpose` now throws an `InvalidOperationException` when the rows do not all have the same
  length, instead of silently producing a wrongly shaped result. The behaviour for jagged input was
  documented as undefined. (#895)
* Fix: calling `Split(separator)` on an array in C# 14 (the default for .NET 10) bound to
  `MemoryExtensions.Split(ReadOnlySpan<T>, T)` because of first-class span conversions, so the
  result was a `SpanSplitEnumerator<T>` instead of an `IEnumerable<IReadOnlyList<T>>` and the
  following LINQ call failed to compile. An array overload of `Split` now keeps arrays bound to
  Funcky. (#893)
* Fix: `Merge` on `IEnumerable<T>` and `IAsyncEnumerable<T>` was not stable: when elements from
  different sources compared equal, the element from the later source was yielded first. Equal
  elements now keep the order of the sources, like a stable sort. (#891)
* Fix: `WithIndex`, `WithFirst`, `WithLast` and `WithPrevious` on an `IList<T>` source returned a
  collection whose `CopyTo` skipped the first `arrayIndex` elements of the source, so copying it
  into an array at a non-zero offset (e.g. via `List<T>.AddRange` or `InsertRange` on a non-empty
  list) dropped elements and left default values behind. (#889)
* Fix: awaiting an empty `Option<Task>`, `Option<Task<T>>`, `Option<ValueTask>` or `Option<ValueTask<T>>`
  via the awaiter's `OnCompleted` method never invoked the continuation. The `await` keyword was not
  affected, because the compiler checks `IsCompleted` first, but custom schedulers and other
  awaiter-based code could hang forever.
* The `Option` awaiters now implement `ICriticalNotifyCompletion`, so `await` no longer needs to
  flow the `ExecutionContext` when the underlying task awaiter does not either.
* Add a `net10.0` target to `Funcky.Async` that only forwards all its types to `Funcky`.
  Projects that reference both `Funcky` and `Funcky.Async` no longer get ambiguous type errors
  (`CS0433`) when they move to .NET 10. On .NET 10 you can simply remove the `Funcky.Async` reference.
* Fix: `Interleave` and `Merge` for `IAsyncEnumerable` now pass the cancellation token to the inner
  enumerators (same fix as in Funcky 3.6.0).

## Funcky 3.6.0

This update is mainly to update to .NET 10.

* Funcky now targets `net10.0`.
* On .NET 10 and newer, the `IAsyncEnumerable` extensions, `AsyncSequence`, `AsyncFunctional` and the
  async `Option`/`Result`/`Either` extensions are part of the `Funcky` package itself, because
  `System.Linq.AsyncEnumerable` is now part of the base class library. Funcky does not depend on
  `System.Linq.Async` on this framework.
* `Funcky.Async` stays the home of these APIs on .NET 9 and older frameworks, where it continues to
  depend on `System.Linq.Async`.
* The `Chunk(int)` overload for `IAsyncEnumerable` is not provided on .NET 10; the identically named
  method in the base class library is used instead and returns arrays rather than `IReadOnlyList<T>`.
  The `Chunk` overloads with a `resultSelector` are unchanged.
* `AdjacentGroupBy` for `IAsyncEnumerable` returns `IGrouping<TKey, TElement>` on .NET 10
  instead of the `IAsyncGrouping<TKey, TElement>` type from `System.Linq.Async`.

## Funcky 3.5.1 | Funcky.Async 1.4.1 | Funcky.Xunit 2.1.1 | Funcky.Analyzers 1.4.1

This is a patch release which fixes vulnerability warnings of (direct and transitive) dependencies.

## Funcky 3.5.0 | Funcky.Async 1.4.0 | Funcky.Xunit 2.1.0 | Funcky.Analyzers 1.4.0
This update is mainly to update to .NET 9 but also has several smaller improvements.

### .NET 9
Thanks to the new `[OverloadResolutionPriority]` attribute, we've been able finally fix
the ambiguous call errors when using `DictionaryExtensions.GetValueOrNone`:

```csharp
Dictionary<string, string> favouriteFoods = new();
Option<int> tausFavouriteFood = favouriteFoods.GetValueOrNone("Tau");
//                                             ^^^^^^^^^^^^^^
// This used to produce a `error CS0121: The call is ambiguous between ...` error
// and now simply works 🎉
```

In good old fashion, we've also added new parse extensions:
* `ParseExtensions.ParseAssemblyNameInfoOrNone`
* `ParseExtensions.ParseTypeNameOrNone`

### Policy for EOL Target Frameworks
We've [explicitly written down](SupportPolicy.md) our policy for how we deal
with older / EOL target frameworks.

In short: EOL target frameworks are supported by Funcky until the next major version.
However, we may no longer test Funcky against those target frameworks, so we
can't guarantee that everything works smoothly.

## New APIs
In Funcky 3.0 we've changed `CycleRange` and `RepeatRange` to return an `IBuffer`
to enable us to memoize the collection, preventing unnecessary enumeration. This however
is unpractical for uses where you want to cycle an already materialized collection.
This release adds two new methods to accommodate that use case:
`CycleMaterialized` and `RepeatMaterialized`.

For convenient downcasting of `Option<T>`, `Result<T>` and `Either<TLeft, T>` values, we've added a new
`DownCast` class:

```csharp
Option<object> option = Option.Some("hello world");
Option<string> downcasted = DownCast<string>.From(option);
```

### Funcky.XUnit
`Funcky.XUnit` has been updated to the latest version (2.9.3) of xUnit.net.

Due to changes in xUnit we've had to change the exception type thrown by our `FunctionalAssert` methods
from `AssertActualExpectedException` to `XunitException`.

### Funcky.Analyzers
The new analyzer rule `λ1009` prevents you from accidentally `default`-instantiating
`Either`, `Result` and `EitherOrBoth`.

### Funcky.Async
The `PowerSet` and `WithPrevious` extensions now correctly pass along the cancellation token.


## Funcky 3.4.0 | Funcky.Async 1.3.0 | Funcky.XUnit 2.0.2

This update is mainly to update to .NET 8 but also has several smaller improvements.

### Native AOT

Both Funcky and Funcky.Async have been annotated to be compatible with [Native AOT](https://learn.microsoft.com/en-gb/dotnet/core/deploying/native-aot).
The only exception is `OptionJsonSerializer` which is not compatible with Native AOT.

### .NET 8

We use the new C#12 and .NET features in the code, and expose new features through our API.

* .NET 8 added new overloads to their `TryParse` APIs. These changes  are reflected in Funcky's `ParseOrNone` APIs.
  * `ParseByteOrNone` overloads with `ReadOnlySpan<byte>` and `string?`
  * `ParseSByteOrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseSingleOrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseDoubleOrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseDecimalOrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseInt16OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseInt32OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseInt64OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseUInt16OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseUInt32OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseUInt64OrNone` overloads with `ReadOnlySpan<byte>`
  * `ParseNumberOrNone<TNumber>` overloads
  * `ParseOrNone<TParsable>` overloads

### String Extensions

We implemented a few of the IEnumerable extensions which are very useful on strings.

* `Chunk` on `string`.
* `SlidingWindow` on `string`.

### Monads

* Implemented `UpCast` for the monds `Option`, `Either`, `Result` and `System.Lazy`.
* Implemented `InspectEmpty` on `IEnumerable` and `IAsyncEnumerable`
* Implemented `ToAsyncEnumerable` extension on `Option`

### IEnumerator

* `MoveNextOrNone` extension on `IEnumerator<T>`

### Consistency

* `FindIndexOrNone` and `FindLastIndexOrNone` extensions on `List`


## Funcky 3.3.0 | Funcky.Analyzers 1.3.0 | Funcky.Xunit 2.0.1
This is a relatively minor release focuses on convenience for our monads `Option`, `Either` and `Result`.

### `GetOrElse` and `OrElse` for all
We've added `GetOrElse` and `OrElse` to `Either` and `Result` bringing them on par with `Option`. \
The corresponding analyzer now also correctly suggests using these methods instead of `Match` for `Result` and `Either`.

### `Inspect` for the error case
All three alternative monads `Option`, `Either` and `Result` now support inspecting the «error» case:
* `Option.InspectNone` - executes a side effect only when the option is `None`.
* `Either.InspectLeft` - executes a side effect only when the either is on the `Left` side.
* `Result.InspectError` - executes a side effect only when the result is an `Error`.

These methods are particularly useful for logging warnings/errors.

### Funcky.XUnit

* Funcky.XUnit is only compatible with XUnit 2.4, this is now correctly declared.

## Funcky 3.2.0 | Funcky.Async 1.2.0
### List Pattern for Option
We've added support for C# 11's List Patterns to `Option<T>`.
This means that you can use regular `switch` expressions / statements to match on options:

```cs
var greeting = person switch
{
    { FirstName: var firstName, LastName: [var lastName] } => $"Hello {firstName} {lastName}",
    { FirstName: var firstName } => $"Hi {firstName}",
};

record Person(string FirstName, Option<string> LastName);
```

### Discard
The new `Discard.__` field provides a short-hand for `Unit.Value` to be used with `switch` expressions.

```cs
using static Funcky.Discard;

return __ switch
{
    _ when user.IsFrenchAdmin() => "le sécret",
    _ when user.IsAdmin() => "secret",
    _ => "(redacted)",
};
```

### Retry with Exception
We've added overloads to the `Retry` and `RetryAsync` functions that allow retrying a function
as long as an exception is thrown.

Example from [`IoRetrier`](https://github.com/messerli-informatik-ag/io/blob/main/IO/Retrier.cs):
```cs
// Retries an action until the file is no longer in use.
Task RetryWhileFileIsInUseAsync(IRetryPolicy policy, Action action)
    => RetryAsync(ActionToUnit(action), exception => exception is IOException && exception.HResult == FileInUseHResult, policy);
```

### New Extensions
* `EnumerableExtensions.MinByOrNone`
* `EnumerableExtensions.MaxByOrNone`
* `ImmutableListExtensions.IndexOfOrNone`
* `ImmutableListExtensions.LastIndexOfOrNone`
* `ListExtensions.IndexOfOrNone`

## Funcky 3.1.0 | Funcky.Async 1.1.0 | Funcky.Analyzers 1.2.0
### New APIs
* ✨ `OptionExtensions.ToNullable` ✨
* `StreamExtensions.ReadByteOrNone`
* New overloads for `ElementAtOrNone` that take an `Index`.
* New overload for `JoinToString` that takes an `IEnumerable<string>`.

### .NET 7
* .NET 7 added new overloads to their `TryParse` APIs. These changes
   are reflected in Funcky's `ParseOrNone` APIs.
* The `ParseOrNone` methods include the new `[StringSyntax]` attribute from .NET 7.

### Analyzers
The new `Option.Match` analyzer suggests simpler alternatives over custom `Match`es including
the all-new `ToNullable` extension.

## Funcky 3.0.0 | Funcky.Async 1.0.0 | Funcky.XUnit 2.0.0
There's a handy [Migration Guide](https://polyadic.github.io/funcky/migration-guide.html) available.

### New APIs
* `Result.GetOrThrow`
* `EnumerableExtensions.GetNonEnumeratedCountOrNone`

#### `PriorityQueue`
* `PriorityQueueExtensions.DequeueOrNone`
* `PeekOrNone`

#### Traversable
The new `Traverse` and `Sequence` extension methods allow you to
«swap» the inner and outer monad (e.g. `Result<Option<T>>` -> `Option<Result<T>>`)

#### `Memoize`
The new `Memoize` extension function returns an `IBuffer` / `IAsyncBuffer`. \
This new type represents ownership over the underlying enumerator (and is therefore `IDisposable`).

`CycleRange` and `RepeatRange` have also been changed to return an `IBuffer`.

### `ParseExtensions`
The parse extensions have been improved with the goal of aligning more with the BCL.
Many of the changes are breaking.

* The functions now use BCL type names instead of C# type names
  (e.g. `ParseIntOrNone` has been renamed to `Parse`)
* The parameter names and nullability have been changed to align with the BCL.
* Added `HttpHeadersNonValidatedExtensions`

### `IReadOnlyList` / `IReadOnlyCollection`
Funcky now communicates materialization in the `IEnumerable<T>` extensions by returning
`IReadOnlyList` or `IReadOnlyCollection`. This reduces «multiple enumeration» warnings.

* `Materialize`
* `Chunk`
* `Partition`
* `Shuffle`
* `SlidingWindow`
* `Split`
* `Transpose`
* `Sequence.Return`

### Disallowing `null` Values
Our `Option<T>` type has always disallowed `null` values.
This has been extended to our other monads: `Result<T>`, `Either<L, R>` and `Reader<E, R>`.

### Breaking Changes
* `Option.None()` has been changed to a property. There's an automatic fix available for this.
* Our `Match` functions now differentiate between `Func` and `Action`.
  The `Action` overloads have been renamed to `Switch`.
* The return type of `EnumerableExtensions.ForEach` has been changed to `Unit`.
* Many parameter names and generic type names have been renamed in an attempt to unify naming across Funcky.
* All `Action` extensions have been moved to a new class `ActionExtensions`.
* `EitherOrBoth` has been moved to the `Funcky` namespace.
* The retry policies have been moved to the `Funcky.RetryPolicies` namespace.
* `Partition` returns a custom `Partitions` struct instead of a tuple.

#### Obsoleted APIs Removed
APIs that have been obsoleted during 2.x have been removed:

* `ObjectExtensions.ToEnumerable`
* `Funcky.GenericConstraints.RequireClass` and `RequireStruct`
* All `Try*` APIs (`TryGetValue`, `TryParse*`, etc.). These APIs use the `OrNone` suffix instead.
* `Sequence.Generate` has been superseded by `Sequence.Successors`
* `CartesianProduct`

#### JSON Converter
We have removed the implicit `System.Text.Json` converter for `Option<T>`.
This means that you'll have to register the `OptionJsonConverter` yourself. \
⚠️ Test this change carefully as you won't get an error during compilation, but rather at runtime.

#### Moved to Funcky.Async
All APIs related to `IAsyncEnumerable` and `Task` have been moved to the new `Funcky.Async` package:

* `AsyncEnumerableExtensions`
* `Functional.RetryAsync` -> `AsyncFunctional.RetryAsync`
* `Option<Task>` and `Option<ValueTask>` awaiters

### Funcky.Async
#### `AsyncSequence`
This class exposes all of the same factory functions as `Sequence`, but for `IAsyncEnumerable`:
* `Return`
* `Successors`
* `Concat`
* `Cycle`
* `CycleRange`
* `FromNullable`
* `RepeatRange`

#### New `IAsyncEnumerable` extensions
We've worked hard towards the goal of parity between our extensions for `IEnumerable` and `IAsyncEnumerable`:

* `AdjacentGroupBy`
* `AnyOrElse`
* `AverageOrNoneAsync` / `MaxOrNoneAsync` / `MinOrNoneAsync`
* `Chunk`
* `ConcatToStringAsync`
* `ExclusiveScan`
* `InclusiveScan`
* `Inspect`
* `Interleave`
* `Intersperse`
* `JoinToStringAsync`
* `MaterializeAsync`
* `Memoize`
* `Merge`
* `NoneAsync`
* `PartitionAsync`
* `PowerSet`
* `Sequence` / `SequenceAsync` / `Traverse` / `TraverseAsync`
* `ShuffleAsync`
* `SlidingWindow`
* `Split`
* `Transpose`
* `WhereNotNull`
* `WithIndex` / `WithLast` / `WithPrevious` / `WithFirst`
* `ZipLongest`

### Funcky.Xunit
* Breaking: The `Is` prefix has been dropped from assertion methods for consistency with
XUnit's naming scheme for assertion methods.

## Funcky 2.7.1
### Deprecations
* `Option.None<T>()`: We originally introduced the `Option.None<T>` method as a future proof replacement to `Option<T>.None` for use in method groups,
  because Funcky 3 changes `Option<T>.None` to a property. This turned out to be confusing to users especially because both method are always suggested in autocomplete.

## Funcky 2.7.0 | Funcky.XUnit 1.0.0 | Funcky.Analyzers 1.1.0
This release is the last non-breaking release for Funcky before 3.0.

### Deprecations
* `EnumerableExtensions.CartesianProduct` will be removed in Funcky 3.
* To align our naming with that of the BCL, the `ParseOrNone` methods
  that return a type that has a keyword in C# `int`, `long`, etc. use the name of the BCL type instead. \
  Example: `ParseIntOrNone` becomes `ParseInt32OrNone`. \
  The old methods will be removed in Funcky 3.
* In preparation for Funcky 3 we deprecated `Option<T>.None` when used as method group. Use `Option.None<T>` instead.

### New `ParseOrNone` extensions
With the help of a source generator we have added a lot of new ParseOrNone methods for various types from the BCL:
  * Unsigned integer types
  * `DateOnly`, `TimeOnly`
  * `Version`
  * Support for `ReadOnlySpan<T>` as input
  * ... and more

### Convenience for `Either` and `Result`
* Added implicit conversions for `Either` and `Result`.
* Implement `Inspect` for `Either` and `Result`.
* Added `Partition` for `IEnumerable<Either>` and `IEnumerable<Result>`.
* Added `ToString` on `Either` and `Result`.
* Implement `ToEither` on `Option`.

### `IEnumerable<T>` extensions
* `AnyOrElse`
* Prefix sum: `InclusiveScan` and `ExclusiveScan`

### Analyzers
This release adds two new analyzer rules:

* λ1003: Warning when certain methods, such as `Match` are used without argument labels
* λ1004: Warning that suggests `.ConcatToString()` over `.JoinToString("")`

Both of these warnings come with corresponding code fixes.

### Funcky.Xunit
* Breaking: Funcky.Xunit now uses the `Funcky` namespace, instead of `Funcky.Xunit`.
* Add assertion methods for testing `Either`: `FunctionalAssert.IsLeft` and `FunctionalAssert.IsRight`.

## Funcky 2.6.0 | Funcky.Analyzers 1.0.0
### Analyzers
This release comes with a new package `Funcky.Analyzers`, which we'll use
to guide users of Funcky

### New extensions
* Add extensions `DequeueOrNone` and `PeekOrNone` on `Queue` and `ConcurrentQueue`.
* Add extension `ConcatToString` as an alias for `string.Concat`.
* Add overload to `WhereSelect` with no parameter.
* Add methods to convert from `Either` to `Option`: [#439](https://github.com/polyadic/funcky/issues/439)
    * `LeftOrNone`: Returns the left value or `None` if the either value was right.
    * `RightOrNone`: Returns the right value or `None` if the either value was left.
* Extension functions for `System.Range` to allow the generations of `IEnumerable<T>`s from Range-Syntax:
  ```cs
  foreach(var i in 1..5) { }

  // negative numbers are not supported
  from x in 5..2
  from y in 1..3
  select (x, y)
  ```

### Improvements to `Sequence`
* `Sequence.Return` now accepts multiple parameters:
  ```cs
  Sequence.Return(1, 2, 3)
  ```
* ⚠️ `Sequence.Generate` has been deprecated in favour of the newly added `Sequence.Successors` function
  which includes the first element (seed) in the generated sequence.

### Improvements to `Option`
* Add `Option.FromBoolean` to create an `Option<T>` from a boolean.

### Improvements to `Result`
The behaviour of the `Result.Error` constructor has been changed regarding exceptions
with an already set stack trace. The original stack trace is now preserved.
Previously this resulted in the stacktrace being replaced (.NET < 5.0) or an error (.NET ≥ 5.0).

### Improvements to `Either`
* Add `Either.Flip` to swaps left with right.

### Tooling
* Funcky automatically adds global usings for the most important namespaces of funcky
  when the `FunckyImplicitUsings` property is set. This requires .NET SDK ≥ 6.0 and C# ≥ 10.0.
* Funcky now supports [trimming](https://docs.microsoft.com/en-us/dotnet/core/deploying/trimming/trim-self-contained) for self-contained deployments.
* `Option<T>` now works with the new [System.Text.Json source generation](https://docs.microsoft.com/en-us/dotnet/standard/serialization/system-text-json-source-generation?pivots=dotnet-6-0).
* The `Funcky` package now supports [Source Link](https://docs.microsoft.com/en-us/dotnet/standard/library-guidance/sourcelink) and deterministic builds.
* The symbols package is now finally working again.

## Funcky 2.5.0
### Reader Monad
This release includes the `Reader` monad including a bunch of factory methods
and convenience extensions.

```cs
public static Reader<Environment, IEnumerable<string>> DefaultLayout(IEnumerable<DateTime> month)
    => from colorizedMonthName in ColorizedMonthName(month)
       from weekDayLine in WeekDayLine()
       from weeksInMonth in month
        .GroupBy(GetWeekOfYear)
        .Select(FormatWeek)
        .Sequence()
       select BuildDefaultLayout(colorizedMonthName, weekDayLine, weeksInMonth);
```

### Improved `Action` Extensions
Funcky now supports `Curry`, `Uncurry` and `Flip` for `Action`s too. \
This release also adds the inversion of `ActionToUnit`: `UnitToAction`

### More Extensions for `IEnumerable<T>`
* `Intersperse`: Adds a given item in between all items of an enumerable.
* `JoinToString`: Alias for `string.Join`.
* `WithPrevious`: Similar to `WithFirst/Last/Index` but with the predecessor of each item.
* `ForEach`: Add an overload to `ForEach` that accepts a `Unit`-returning `Func`.

### Additional Factory Methods
* `EitherOrBoth.FromOptions` creates an `EitherOrBoth` from two options.
* `Lazy.FromFunc` creates a `Lazy<T>` from a `Func`. \
   This is sugar over the `Lazy<T>` constructor, with the additional benefit of supporting type inference.
* `Lazy.Return` creates a `Lazy<T>` from a value. \
  This is sugar over the `Lazy<T>` constructor, with the additional benefit of supporting type inference.

### Documentation Improvements
This release comes with a few small documentation improvements.
Funcky users will now also see the `[Pure]` attributes which were previously not emitted.

## Funcky 2.4.1
* Remove upper bounds on all Microsoft.Bcl.\* dependencies.
  Between the 2.3.0 and 2.4.0 release an overly restrictive upper bound was accidentally introduced for Microsoft.Bcl.AsyncInterfaces.

## Funcky 2.4.0
### `Try*` → `*OrNone`
We've renamed all `Try*` methods, such as `TryParse`, `TryGet` value to `*OrNone`.
The old methods are still available, but marked as obsolete and will be removed in 3.0.0.

### Factory methods for `IEnumerable<T>`
This release adds some new factory methods for creating `IEnumerable<T>`
to the `Sequence` class:
* `Sequence.RepeatRange`: Generates a sequence that contains the same sequence of elements the given number of times
* `Sequence.Cycle`: Cycles the same element over and over again as an endless generator.
* `Sequence.CycleRange`: Generates a sequence that contains the same sequence of elements over and over again as an endless generator
* `Sequence.Concat`

### More Extension Methods
#### for `IEnumerable<T>`
  * `Materialize`: Materializes all the items of a lazy enumerable.
  * `PowerSet`: Returns a sequence with the set of all subsets
  * `Shuffle`: Returns the given sequence in random Order in O(n).
  * `Split`: Splits the source sequence a separator.
  * `ZipLongest`: Zips two sequences with different lengths.
#### for `string`
* `SplitLazy`: Splits a string by separator lazily.
* `SplitLines`: Splits a string by newline lazily.
#### for `Func`
* `Curry`
* `Uncurry`
* `Flip`
* `Compose`

### `EitherOrBoth`
EitherOrBoth is a new data type that can represent `Left`, `Right` and `Both`. It is used in `ZipLongest`.

### `Monad.Return`
This release adds a `Return` method for all monad types in Funcky:
* `Option.Return`
* `Either<TLeft>.Return`
* `Result.Return`

### `OptionEqualityComparer`
To support more advanced comparison scenarios, `OptionEqualityComparer` has been added similar to the already existing `OptionComparer`.

### Smaller Improvements
* Added a missing `Match` overload to `Either` that takes `Action`s
* Added additional overloads for `Functional.True` and `Functional.False` for up to four parameters.

## Funcky 2.3.0
* `net5.0` has been added to Funcky's target frameworks.

### Improvements to `Option<T>`
* `Option<T>` is now implicitly convertible from `T`.
  ```csharp
  public static Option<int> Answer => 42;
  ```
* `Option` adds support for `System.Text.Json`:\
  The custom `JsonConverter` is picked up automatically when serializing/deserializing.
  `None` is serialized as `null` and `Some(value)` is serialized to whatever `value` serializes to.

### Factory methods for `IEnumerable<T>`
This release adds factory methods for creating `IEnumerable<T>`
with the static class `Sequence`:
* `Sequence.Return`: Creates an `IEnumerable<T>` with exactly one item.
* `Sequence.FromNullable`: Creates an `IEnumerable<T>` with zero or one items.
* `Sequence.Generate`: Creates an `IEnumerable<T>` using a generation function and a seed.

### More Extension Methods for `IEnumerable<T>`
This release adds a bunch of new extension methods on `IEnumerable<T>`:
* `AdjacentGroupBy`
* `AverageOrNone`
* `CartesianProduct`
* `Chunk`
* `ElementAtOrNone`
* `Interleave`
* `MaxOrNone`
* `Merge`
* `MinOrNone`
* `Pairwise`
* `Partition`
* `SlidingWindow`
* `TakeEvery`
* `Transpose`
* `WithFirst`
* `WithIndex`
* `WithLast`

### `IAsyncEnumerable<T>` Support
This release adds a couple of extension methods that provide interoperability
with `Option<T>` to `IAsyncEnumerable<T>`:
* `WhereSelect`
* `FirstOrNoneAsync`
* `LastOrNoneAsync`
* `SingleOrNoneAsync`
* `ElementAtOrNoneAsync`

A couple of the new extension methods on `IEnumerable<T>` have async counterparts:
* `Pairwise`
* `TakeEvery`

The naming of the extension methods and their overloads follows that of [`System.Linq.Async`](https://github.com/dotnet/reactive/tree/main/Ix.NET/Source/System.Linq.Async).

### Improved `IQueryable` Support
This release adds specialized extension methods for `IQueryable<T>` that are better
suited especially for use with EF Core:
* `FirstOrNone`
* `LastOrNone`
* `SingleOrNone`

### Dependencies
To support .NET Standard, Funcky conditionally pulls in dependencies
that provide the missing functionality:
* `Microsoft.Bcl.AsyncInterfaces` for .NET Standard 2.0
* `System.Collections.Immutable` and `System.Text.Json` for .NET Standard 2.0 and 2.1
* The version constraints for all these packages have been relaxed to allow 5.x.

### Improvements
* `ConfigureAwait(false)` is now used everywhere `await` is used.
* The `IRetryPolicy` implementations now use correct `Timespan` with `double` multiplication
  when targeting .NET Standard 2.0.

### Deprecations
* `ObjectExtensions.ToEnumerable` has been deprecated in favor of `Sequence.FromNullable`.
* `RequireClass` and `RequireStruct` have been obsoleted with no replacement.

## Funcky 2.2.0 | Funcky.xUnit 0.1.3
* Added overload to `Functional.Retry` with a `IRetryPolicy`.
* Added `None` overload that takes no predicate.

## Funcky 2.1.1 | Funcky.xUnit 0.1.2
* Re-release of previous release with correct assemblies.

## Funcky 2.1.0 | Funcky.xUnit 0.1.1
* Add `Inspect` method to `Option` akin to `IEnumerable.Inspect`.
* Add `ToTheoryData` extension for `IEnumerable<T>` for xUnit.
* Add `Unit.Value` as a way to a get a `Unit` value.
* Add `Functional.Retry` which retries a producer until `Option.Some` is returned.

## Funcky 2.0.0
### Breaking Changes
* Remove `Reader` monad based on `await`.
* Remove `IToString`.
* Remove overload for `Option.From` that flattens passed `Option`s.
* Move `ToEnumerable` extension method to its own class.
  This is only a breaking change if you've used the extension method as normal method.
  In that case you need to change `EnumerableExtensions.ToEnumerable` to `ObjectExtensions.ToEnumerable`.
* Rename `Option.From` to `Option.FromNullable` and remove overload that takes non-nullable value types.
* Unify `Option<T>.ToEnumerable` and `Yield` to `ToEnumerable`
* Rename `OrElse` overloads that return the item to `GetOrElse` which improves overload resolution.
* The `Each` extension method on `IEnumerable<T>` has been renamed to `ForEach`.
* Move the `Ok` constructor of `Result<T>` to a non-generic class. This allows for the compiler to infer the generic type.
  Old: `Result<int>.Ok(10)`. New: `Result.Ok(10)`.
* Use `Func<T, bool>` instead of `Predicate<T>` in predicate composition functions (`Functional.All`, `Functional.Any`, `Functional.Not`),
  because most APIs in `System` use `Func`.
* `Functional.Any` now returns `false` when the given list of predicates is empty.

### Fixes
* Fix incorrect `Equals` implementation on `Option`.
  `Equals` previously returned `true` when comparing a `None` value with a `Some` value containing the default value of the type.
* `Exception` created by `Result` monad contains valid stack trace
* Fix incorrect implementation on `Result.SelectMany` which called the `selectedResultSelector` even when the
  result was an error. As a result (pun intended) of the fix, `ResultCombinationException` is no longer needed and also removed.

### Additions
* Add `IndexOfOrNone`, `LastIndexOfOrNone`, `IndexOfAnyOrNone` and `LastIndexOfAnyOrNone` extension methods to `string`.
* Added `Curry`, `Uncurry` and `Flip` to the `Functional` Class
* Add extension method for `HttpHeaders.TryGetValues`, which returns an `Option`.
* Add extension methods for getting `Stream` properties that are not always available, as `Option`:
  `GetLengthOrNone`, `GetPositionOrNone`, `GetReadTimeoutOrNone`, `GetWriteTimeoutOrNone`.
* Add `None` extension method to `IEnumerable`.
* `Option<Task<T>>`, `Option<Task>` and their `ValueTask` equivalents are now awaitable:
  ```csharp
  var answer = await Option.Some(Task.FromResult(42));
  ```

### Improvements
* Full nullable support introduced with C# 8.
* Mark our functions as `[Pure]`.
* Implement `IEquatable` on `Option`, `Result` and `Either`.

## Funcky 2.0.0-rc.2
* Move the `Ok` constructor of `Result<T>` to a non-generic class. This allows for the compiler to infer the generic type.
  Old: `Result<int>.Ok(10)`. New: `Result.Ok(10)`.
* Add `IndexOfOrNone`, `LastIndexOfOrNone`, `IndexOfAnyOrNone` and `LastIndexOfAnyOrNone` extension methods to `string`.
* Rename `OrElse` overloads that return the item to `GetOrElse` which improves overload resolution.
* Added `Curry`, `Uncurry` and `Flip` to the `Functional` Class
* Remove `IToString`.
* Mark our functions as `[Pure]`.
* Fix incorrect implementation on `Result.SelectMany` which called the `selectedResultSelector` even when the
  result was an error. As a result (pun intended) of the fix, `ResultCombinationException` is no longer needed and also removed.

## Funcky 2.0.0-rc.1
* Full nullable support introduced with C# 8
* Rename `Option.From` -> `Option.FromNullable` and remove overload that takes non-nullable value types.
* Use `Func<T, bool>` instead of `Predicate<T>` in predicate composition functions (`Functional.All`, `Functional.Any`, `Functional.Not`),
  because most APIs in `System` use `Func`.
* `Functional.Any` now returns `false` when the given list of predicates is empty.
* The `Each` extension method on `IEnumerable<T>` has been renamed to `ForEach`.
* Unify `Option<T>.ToEnumerable` and `Yield` to `ToEnumerable`
* Remove `Reader` monad based on `await`.
* `Exception` created by `Result` monad contains valid stack trace

## Funcky 1.8.0
* Added overload for `AndThen` which flattens the `Option`
* Add `Where` method to `Option<T>`, which allows filtering the `Option` by a predicate.
* Add overload for `Option<T>.SelectMany` that takes only a selector.
* Add `WhereNotNull` extension method for `IEnumerable<T>`.

## Funcky 1.7.0
* Add nullability annotations to everything except for `Monads.Reader`.
* Add a function for creating an `Option<T>` from a nullable value: `Option.From`.
* `Either.Match` now throws when called on an `Either` value created using `default(Either<L, R>)`.
* Add `True` and `False` functions to public API
* Match of `Result` Monad accepts actions
* Add `FirstOrNone`, `LastOrNone` and `SingleOrNone` extension functions

## Funcky 1.6.0
* Add ToEnumerable function to `Option<T>`.
* Add `WhereSelect` extension function for `IEnumerable<T>`.
* Add missing overload for nullary actions to `ActionToUnit`.
