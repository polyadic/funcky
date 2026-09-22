// On .NET 10 and newer, Funcky's async APIs are part of the Funcky package itself.
// This asset of Funcky.Async only exists so that projects referencing both packages
// (e.g. after upgrading from an older framework) keep compiling: every public type
// that used to live here is forwarded to its new home in Funcky.
using System.Runtime.CompilerServices;
using Funcky;

[assembly: TypeForwardedTo(typeof(AsyncFunctional))]
[assembly: TypeForwardedTo(typeof(AsyncSequence))]
[assembly: TypeForwardedTo(typeof(IAsyncBuffer<>))]
[assembly: TypeForwardedTo(typeof(AsyncEnumerableExtensions))]
[assembly: TypeForwardedTo(typeof(OptionAsyncExtensions))]
[assembly: TypeForwardedTo(typeof(ResultAsyncExtensions))]
[assembly: TypeForwardedTo(typeof(EitherAsyncExtensions))]
[assembly: TypeForwardedTo(typeof(OptionTaskAwaiter))]
[assembly: TypeForwardedTo(typeof(OptionTaskAwaiter<>))]
[assembly: TypeForwardedTo(typeof(OptionValueTaskAwaiter))]
[assembly: TypeForwardedTo(typeof(OptionValueTaskAwaiter<>))]
[assembly: TypeForwardedTo(typeof(ConfiguredOptionTaskAwaitable))]
[assembly: TypeForwardedTo(typeof(ConfiguredOptionTaskAwaitable<>))]
[assembly: TypeForwardedTo(typeof(ConfiguredOptionValueTaskAwaitable))]
[assembly: TypeForwardedTo(typeof(ConfiguredOptionValueTaskAwaitable<>))]
