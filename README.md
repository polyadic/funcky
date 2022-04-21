# Funcky

Funcky is a functional library for C# which encourages correct usage of the functional programming paradigm.

[![Build](https://github.com/polyadic/funcky/workflows/Build/badge.svg)](https://github.com/messerli-informatik-ag/funcky/actions?query=workflow%3ABuild)
[![Licence: MIT](https://img.shields.io/badge/licence-MIT-green)](https://raw.githubusercontent.com/polyadic/funcky/main/LICENSE-MIT)
[![Licence: Apache](https://img.shields.io/badge/licence-Apache-green)](https://raw.githubusercontent.com/polyadic/funcky/main/LICENSE-Apache)

## Packages

* **Funcky** \
  [![NuGet package](https://buildstats.info/nuget/Funcky)](https://www.nuget.org/packages/Funcky)
* **Funcky.XUnit** \
  [![NuGet package](https://buildstats.info/nuget/Funcky.XUnit)](https://www.nuget.org/packages/Funcky.XUnit)
* **Funcky.Analyzers**: Analyzers to guide to the correct usage of Funcky. \
  [![NuGet package](https://buildstats.info/nuget/Funcky.Analyzers)](https://www.nuget.org/packages/Funcky.Analyzers)
* **[Funcky.EntityFrameworkCore](https://github.com/polyadic/funcky-efcore)**: Provides interoperability between Funcky and EF Core \
  [![NuGet package](https://buildstats.info/nuget/Funcky.EntityFrameworkCore)](https://www.nuget.org/packages/Funcky.EntityFrameworkCore)
  [![git: polyadic/funcky-efcore](https://img.shields.io/badge/git-polyadic%2Ffuncky--efcore-blue)](https://github.com/polyadic/funcky-efcore)
* **[Funcky.DiscriminatedUnion](https://github.com/polyadic/funcky-discriminated-union)**: Provides source generator for discriminated union support in C# \
  [![NuGet package](https://buildstats.info/nuget/Funcky.DiscriminatedUnion)](https://www.nuget.org/packages/Funcky.DiscriminatedUnion)
  [![git: polyadic/funcky-discriminated-union](https://img.shields.io/badge/git-polyadic%2Ffuncky--discriminated--union-blue)](https://github.com/polyadic/funcky-discriminated-union)

## Features

See our in progress [documentation](https://polyadic.github.io/funcky/) for more examples.

### Option Monad

The `Option` monad is the centerpiece of Funcky. It provides a safe and easy structure for working with optional values. \
It follows naming already established by LINQ (`Select`, `SelectMany`, `Where`).

```cs
Option<string> input = ...;

var result = input
    .SelectMany(v => v.ParseInt32OrNone())
    .Where(n => n >= 0)
    .Select(n => $"Non-Zero: {n}");

result.AndThen(Console.WriteLine);
```

### IEnumerable Extensions

Funcky provides a plethora of extensions for `IEnumerable` that help with writing code using Functional programming paradigms.

```cs
Sequence.Return(1, 2, 3, 4)
    .Pairwise((left, right) => left + right) // [3, 5, 7]
    .Intersperse(-1) // [3, -1, 5, -1, 7]
    .Inspect(item => Console.WriteLine(item))
    .SlidingWindow(3) // [[3, -1, 5], [-1, 5, -1], [5, -1, 7]]
    .WhereSelect(window => window.AverageOrNone()) // [2, 1, 3]
    .JoinToString(", "); // "2, 1, 3"
```

### … and more

* Extensions that provide interoperability with `Option` for `IQueryable`, `IAsyncEnumerable`, `string`.
* Fundamental functions: `Identity`, `True`, `False`, etc.
* «Constructors» for `IEnumerable`: `Sequence.Return`, `Sequence.Successors`, etc.
* `Unit` type
* `Result` monad
* `Reader` monad
* `Either` monad

## Motivation

Functional programming is the oldest of the three major programming paradigms, none the less it is the last which gets wide spread usage. Even in languages like C++, Java or C# we want to use a functional style of programming.

Linq is the first Monad which got wide spread use in C#, and most C# programmers were not even aware of it beeing a monad, which probably helped.

Mark Seemann points out that "Unfortunately, Maybe implementations often come with an API that enables you to ask a Maybe object if it's populated or empty, and a way to extract the value from the Maybe container. This misleads many programmers [...]"

https://blog.ploeh.dk/2019/02/04/how-to-get-the-value-out-of-the-monad/

This library is based on his example code, and should grow slowly to a library which helps to use and understand the Functional programming paradigm. Functional programming is side-effect free and the strong type system can be used to make illegal state impossible.

Use functional programming as an additional asset to write correct code.

## Documentation

[![Documentation: in progress](https://img.shields.io/badge/documentation-in%20progress-orange)](https://polyadic.github.io/funcky/)

## Other libraries

There are several libraries available which try to give you more functional features in C#. So

* **Funcky wants to be functional C#.**
* **Funcky tries to use the C# monadic interfaces as an advantage**
* **We do not provide our own record type. Use the new [record types] in C# 9 or a weaver like [Equals.Fody].**

### [LanguageExt](https://github.com/louthy/language-ext)

This library is probably the most complete attempt to functional programming in C#, however it is very opinionated and admits to be not very idiomatic in C#. It certainly is more mature than Funcky and has a lot of features. If you want to go fully functional and for some reason cannot use F# this might be the way to go.

### [Eff](https://github.com/nessos/Eff)

Eff is inspired by the Eff programming language and the implementation of Algebraic Effects. It's only purpose is the handling of side effects and using the await syntax in a very elegant way.

We think the approach is very nice but cumbersome in usage, however we really love the appraoch with the await syntax. The library is very specialised an can be used in combination with any other functional style library.

### [MoreLinq](https://github.com/morelinq/MoreLINQ/)

MoreLinq provides more extension functions on `IEnumerable`, but has no additional functional concepts. We also provide additional extension functions on `IEnumerable`, but we also try to make them work in combination with our Monads and the async Monad. The different Monad-Syntaxes in C# (Linq, async) do not play niceley together.

### … and more

* [Functional.Primitives.Extensions](https://github.com/JohannesMoersch/Functional)
* [Functional.Maybe](https://github.com/AndreyTsvetkov/Functional.Maybe)
* [Tango](https://github.com/gabrielschade/tango)

## Contributing

Contributions are more than welcome. Just open a PR :)
If you want something easy to work on, there are a few issues marked with [good first issue].

### Documentation

To build the documentation you need [mdBook] installed.
When working on the documentation it's useful to have `mdbook` watching and automatically rebuilding on changes:

```bash
mdbook serve Documentation
```

## Dependency Policy

The core `Funcky` package is not allowed to have dependencies. Backwards compatibility packages from Microsoft that are included in
newer framework versions (e.g. [`Microsoft.Bcl.AsyncInterfaces`], [`System.Collections.Immutable`]) are exempt from this rule.

Interoperability with other libraries should be provided in separate packages (e.g. `Funcky.Xunit`, [`Funcky.NewtonsoftJson`])


[good first issue]: https://github.com/polyadic/funcky/labels/good%20first%20issue
[mdBook]: https://github.com/rust-lang/mdBook
[`Funcky.NewtonsoftJson`]: https://github.com/polyadic/funcky-newtonsoftjson
[`Microsoft.Bcl.AsyncInterfaces`]: https://www.nuget.org/packages/Microsoft.Bcl.AsyncInterfaces
[`System.Collections.Immutable`]: https://www.nuget.org/packages/System.Collections.Immutable
[record types]: https://docs.microsoft.com/en-us/dotnet/csharp/tutorials/exploration/records
[Equals.Fody]: https://github.com/Fody/Equals
