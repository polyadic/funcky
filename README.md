# Funcky

Funcky is a functional library for C# which encourages correct usage of the functional programming paradigm.

[![Build](https://github.com/polyadic/funcky/workflows/Build/badge.svg)](https://github.com/polyadic/funcky/actions?query=workflow%3ABuild)
[![Documentation](https://img.shields.io/badge/documentation-stable-brightgreen)](https://polyadic.github.io/funcky/)
[![Licence: MIT](https://img.shields.io/badge/licence-MIT-green)](https://raw.githubusercontent.com/polyadic/funcky/main/LICENSE-MIT)
[![Licence: Apache](https://img.shields.io/badge/licence-Apache-green)](https://raw.githubusercontent.com/polyadic/funcky/main/LICENSE-Apache)

## Documentation

The [Funcky book](https://polyadic.github.io/funcky/) is the place to start. It explains the mindset behind the library,
walks through every monad and every `IEnumerable` extension with examples and marble diagrams, documents all analyzer
rules, and ends with a few case studies. The [changelog](changelog.md) lists what changed in each release and the
[support policy](SupportPolicy.md) which target frameworks are supported.

## Packages

### Core

| Package | | |
| --- | --- | --- |
| **Funcky** | [![NuGet Version](https://img.shields.io/nuget/v/Funcky)](https://www.nuget.org/packages/Funcky) | The library itself: `Option`, `Result`, `Either`, `Reader`, `Unit`, the `IEnumerable` and `string` extensions, the functional helpers and the built-in correctness analyzers. No dependencies. |
| **Funcky.Analyzers** | [![NuGet Version](https://img.shields.io/nuget/v/Funcky.Analyzers)](https://www.nuget.org/packages/Funcky.Analyzers) | Optional analyzers and code fixes that suggest more idiomatic Funcky code. See the [analyzer rules](https://polyadic.github.io/funcky/analyzer-rules/analyzer-rules.html). |
| **Funcky.Async** | [![NuGet Version](https://img.shields.io/nuget/v/Funcky.Async)](https://www.nuget.org/packages/Funcky.Async) | `IAsyncEnumerable` and `Task` support for frameworks older than .NET 10. On .NET 10 and newer these APIs are part of **Funcky** itself and this package only forwards to them. |

### Testing

| Package | | |
| --- | --- | --- |
| **Funcky.Xunit** | [![NuGet Version](https://img.shields.io/nuget/v/Funcky.Xunit)](https://www.nuget.org/packages/Funcky.Xunit) | `FunctionalAssert` for xUnit.net 2.x: assert that an `Option` is `Some`, a `Result` is `Ok` or an `Either` is `Right`, and get the unpacked value back. |
| **Funcky.Xunit.v3** | [![NuGet Version](https://img.shields.io/nuget/v/Funcky.Xunit.v3)](https://www.nuget.org/packages/Funcky.Xunit.v3) | The same assertions for xUnit.net 3.x, plus serializers so `Option`, `Either` and `Unit` can be used as theory data. |

### Interoperability

These packages live in their own repositories.

| Package | | |
| --- | --- | --- |
| **[Funcky.EntityFrameworkCore](https://github.com/polyadic/funcky-efcore)** | [![NuGet Version](https://img.shields.io/nuget/v/Funcky.EntityFrameworkCore)](https://www.nuget.org/packages/Funcky.EntityFrameworkCore) | Interoperability between Funcky and EF Core. |
| **[Funcky.DiscriminatedUnion](https://github.com/polyadic/funcky-discriminated-union)** | [![NuGet Version](https://img.shields.io/nuget/v/Funcky.DiscriminatedUnion)](https://www.nuget.org/packages/Funcky.DiscriminatedUnion) | A source generator for `Match` methods on discriminated unions. **Retiring with Funcky 4.0.0**: C# gets native `union` types and `closed` hierarchies with .NET 11, which Funcky 4.0.0 adopts. The generator keeps working with the 3.x line but receives no new features, see [#900](https://github.com/polyadic/funcky/issues/900). |

## Highlights

### Option: no `Value`, no `HasValue`

The `Option<T>` monad is the centerpiece of Funcky. It represents a value that may be absent, and it deliberately has
no `Value`, `HasValue`, `IsSome` or `GetOrThrow`. Instead of checking and extracting, you tell the option what to do
with the value, using the names LINQ already taught you: `Select`, `SelectMany`, `Where`.

```cs
Option<string> input = ...;

var result = input
    .SelectMany(v => v.ParseInt32OrNone())
    .Where(n => n >= 0)
    .Select(n => $"Non-negative: {n}");

result.Switch(
    none: () => Console.WriteLine("nothing to show"),
    some: Console.WriteLine);
```

The way out at the edges of your program is to say what happens in both cases, with `Match`, `Switch` or `GetOrElse`.
The [Option chapter](https://polyadic.github.io/funcky/option.html) is organized as a ladder of answers to
"how do I get the value out?".

### The TryVerb pattern: `…OrNone` instead of `Try…` with `out`

`TryParse`, `TryGetValue`, `FirstOrDefault`, `Max`: Funcky gives every BCL member that may have no answer an `…OrNone`
counterpart that returns an `Option` instead of a default value or an `out` parameter. The result composes, and the
compiler no longer lets you forget the missing case.

```cs
var port = settings.GetValueOrNone("port")
    .SelectMany(value => value.ParseInt32OrNone())
    .Where(p => p is > 0 and < 65536)
    .GetOrElse(8080);

var oldest = people.MaxByOrNone(person => person.Age);
```

### Result and Either for computations that can fail

`Result<T>` carries either a value or an `Exception`, `Either<L, R>` carries one of two arbitrary types. Both support
the same LINQ operators as `Option`, so a chain of fallible steps reads like a query and the failure is handled once,
at the end.

```cs
Result<Order> order = ParseOrder(json)
    .SelectMany(Validate)
    .SelectMany(Price);

return order.Match(
    ok: o => Results.Ok(o),
    error: e => Results.BadRequest(e.Message));
```

### IEnumerable extensions

Funcky adds the sequence operators LINQ lacks: windowing, pairing, interspersing, partitioning, side effects, and
`…OrNone` variants of every element query. They are lazy where LINQ is lazy and work together with the monads above.

```cs
Sequence.Return(1, 2, 3, 4)
    .Pairwise((left, right) => left + right) // [3, 5, 7]
    .Intersperse(-1) // [3, -1, 5, -1, 7]
    .Inspect(Console.WriteLine)
    .SlidingWindow(3) // [[3, -1, 5], [-1, 5, -1], [5, -1, 7]]
    .WhereSelect(window => window.AverageOrNone()) // [2, 1, 3]
    .JoinToString(", "); // "2, 1, 3"
```

### Analyzers that catch misuse

Funcky ships with built-in analyzers that turn misuse of its types into compile errors, for example instantiating an
`Option` with `default` or treating an option as a collection of more than one element. The optional
**Funcky.Analyzers** package adds style rules with code fixes that lead from the general tool to the specific one, for
example from `option.Match(none: fallback, some: Identity)` to `option.GetOrElse(fallback)`.

### … and more

* `Reader` and `Lazy` monads, and `Unit` as a first-class "no value" type.
* Functional helpers: `Identity`, `Compose`, `Curry`, `Flip`, predicate composition, and `Retry` with configurable retry policies.
* «Constructors» for `IEnumerable`: `Sequence.Return`, `Sequence.Successors`, `Sequence.Cycle`, etc.
* `IAsyncEnumerable` counterparts of the sequence extensions, and `await` support for `Option<Task<T>>`.
* Interoperability with `IQueryable`, `string`, `Stream`, `Nullable<T>` and the numeric types.
* Supports every framework from .NET Standard 2.0 and .NET Framework 4.8 to the newest .NET, and is trimming and
  Native AOT compatible.

## Motivation

Functional programming is the oldest of the three major programming paradigms, nonetheless it is the last to get
widespread usage. Even in languages like C++, Java or C# we want to use a functional style of programming.

LINQ is the first monad which got widespread use in C#, and most C# programmers were not even aware of it being a monad,
which probably helped.

Mark Seemann points out in [How to get the value out of the monad](https://blog.ploeh.dk/2019/02/04/how-to-get-the-value-out-of-the-monad/):

> Unfortunately, Maybe implementations often come with an API that enables you to ask a Maybe object if it's populated
> or empty, and a way to extract the value from the Maybe container. This misleads many programmers [...]

This library is based on his example code and grows slowly into a library which helps to use and understand the
functional programming paradigm. Functional programming is side-effect free, and the strong type system can be used to
make illegal state impossible.

Use functional programming as an additional asset to write correct code.

## Other libraries

There are several libraries available which try to give you more functional features in C#. In comparison:

* **Funcky wants to be functional C#.** It builds on LINQ, lambdas and the type system you already use.
* **Funcky uses the C# monadic interfaces as an advantage.** Query syntax and `await` work with its monads.
* **Funcky does not provide its own record or union types.** Use the [record types] built into C#.

### [LanguageExt](https://github.com/louthy/language-ext)

This library is probably the most complete attempt at functional programming in C#, however it is very opinionated and
admits to being not very idiomatic in C#. It certainly is more mature than Funcky and has a lot of features. If you want
to go fully functional and for some reason cannot use F#, this might be the way to go.

### [Eff](https://github.com/nessos/Eff)

Eff is inspired by the Eff programming language and the implementation of algebraic effects. Its only purpose is the
handling of side effects, using the `await` syntax in a very elegant way.

We think the approach is very nice but cumbersome in usage, however we really love the approach with the `await` syntax.
The library is very specialised and can be used in combination with any other functional style library.

### [MoreLinq](https://github.com/morelinq/MoreLINQ/)

MoreLinq provides more extension functions on `IEnumerable`, but has no additional functional concepts. We also provide
additional extension functions on `IEnumerable`, but we also try to make them work in combination with our monads and
the async monad. The different monad syntaxes in C# (LINQ, async) do not play nicely together.

### … and more

* [Functional.Primitives.Extensions](https://github.com/JohannesMoersch/Functional)
* [Functional.Maybe](https://github.com/AndreyTsvetkov/Functional.Maybe)
* [Tango](https://github.com/gabrielschade/tango)

## License
Licensed under either of

* Apache License, Version 2.0
  ([LICENSE-Apache](LICENSE-Apache) or <http://www.apache.org/licenses/LICENSE-2.0>)
* MIT license
  ([LICENSE-MIT](LICENSE-MIT) or <http://opensource.org/licenses/MIT>)

at your option.

## Contribution
Contributions are more than welcome. Just open a PR :)
If you want something easy to work on, there are a few issues marked with [good first issue].

Unless you explicitly state otherwise, any contribution intentionally submitted
for inclusion in the work by you, as defined in the Apache-2.0 license, shall be
dual licensed as above, without any additional terms or conditions.

### Documentation

To build the documentation you need [mdBook] installed.
When working on the documentation it's useful to have `mdbook` watching and automatically rebuilding on changes:

```bash
mdbook serve Documentation
```

## Dependency Policy

The core `Funcky` package is not allowed to have dependencies. Backwards compatibility packages from Microsoft that are included in
newer framework versions (e.g. [`Microsoft.Bcl.AsyncInterfaces`], [`System.Collections.Immutable`]) are exempt from this rule.

Interoperability with other libraries is provided in separate packages (e.g. `Funcky.Xunit`, `Funcky.EntityFrameworkCore`).


[good first issue]: https://github.com/polyadic/funcky/labels/good%20first%20issue
[mdBook]: https://github.com/rust-lang/mdBook
[`Microsoft.Bcl.AsyncInterfaces`]: https://www.nuget.org/packages/Microsoft.Bcl.AsyncInterfaces
[`System.Collections.Immutable`]: https://www.nuget.org/packages/System.Collections.Immutable
[record types]: https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/record
