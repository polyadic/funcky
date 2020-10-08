# Funcky

Funcky is a functional Library for C# which encourages correct usage of the functional programming paradigm.

[![Build](https://github.com/polyadic/funcky/workflows/Build/badge.svg)](https://github.com/messerli-informatik-ag/funcky/actions?query=workflow%3ABuild)
[![Licence: MIT](https://img.shields.io/badge/licence-MIT-green)](https://raw.githubusercontent.com/polyadic/funcky/master/LICENSE-MIT)
[![Licence: Apache](https://img.shields.io/badge/licence-Apache-green)](https://raw.githubusercontent.com/polyadic/funcky/master/LICENSE-Apache)

## Packages

### Funcky

[![NuGet package](https://buildstats.info/nuget/Funcky)](https://www.nuget.org/packages/Funcky)

### Funcky.XUnit

[![NuGet package](https://buildstats.info/nuget/Funcky.XUnit)](https://www.nuget.org/packages/Funcky.XUnit)

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
* **We do not provide our own RecordType. Use Fody or wait for C# 9**

### [LanguageExt](https://github.com/louthy/language-ext)

This library is probably the most complete attempt to functional programming in C#, however it is very opinionated and admits to be not very idiomatic in C#. It certainly is more mature than Funcky and has a lot of features. If you want to go fully functional and for some reason cannot use F# this might be the way to go.

### [Eff](https://github.com/nessos/Eff)

Eff is inspired by the Eff programming language and the implementation of Algebraic Effects. It's only purpose is the handling of side effects and using the await syntax in a very elegant way.

We think the approach is very nice but cumbersome in usage, however we really love the appraoch with the await syntax. The library is very specialised an can be used in combination with any other functional style library.

### [MoreLinq](https://github.com/morelinq/MoreLINQ/)

MoreLinq provides more extension functions on `IEnumerable`, but has no additional functional concepts. We also provide additional extension functions on `IEnumerable`, but we also try to make them work in combination with our Monads and the async Monad. The different Monad-Syntaxes in C# (Linq, async) do not play niceley together.

### [Galaxus.Functional](https://github.com/DigitecGalaxus/Galaxus.Functional)

This is a very simple Functional Library with similar ideas in spirit but not as mature.

### And more…

* [Functional.Primitives.Extensions](https://github.com/JohannesMoersch/Functional)
* [Functional.Maybe](https://github.com/AndreyTsvetkov/Functional.Maybe)
* [Tango](https://github.com/gabrielschade/tango)


## Features

See our [documentation](https://polyadic.github.io/funcky/) (still in progress)

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
