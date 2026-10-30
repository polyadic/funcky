## About
Provides async counterparts for [Funcky]'s `IEnumerable` extensions,
`IEnumerable` generators, traversal and retrying for frameworks older than .NET 10.

Starting with .NET 10, `IAsyncEnumerable` LINQ is part of the base class library and
these APIs are part of the [Funcky] package itself. The `net10.0` and `net11.0` assets of this package
only forward their types to `Funcky`, so existing projects keep compiling. Once you target
.NET 10 or newer, you can remove the reference to `Funcky.Async`.

On older frameworks the package builds on [System.Linq.AsyncEnumerable](https://www.nuget.org/packages/System.Linq.AsyncEnumerable),
the `IAsyncEnumerable` LINQ of .NET 10 as a package, so the operators have the same shapes everywhere. A reference to
[System.Linq.Async](https://www.nuget.org/packages/System.Linq.Async) 5 or 6 next to it clashes (CS0433); upgrade it to
System.Linq.Async 7 or drop it. Projects that cannot stay on Funcky.Async 1.x.

## Main Types
* `Funcky.AsyncSequence`—Generate asynchronous sequences.
* `Funcky.Extensions.AsyncEnumerableExtensions`—Extensions for `IAsyncEnumerable`.
* `Funcky.AsyncFunctional`—Async Retrying.

## Feedback & Contributing
This package is released as open source under the MIT or Apache-2.0 license at your choice.
Bug reports and contributions are welcome at the [GitHub repository](https://github.com/polyadic/funcky).

[Funcky]: https://www.nuget.org/packages/Funcky
