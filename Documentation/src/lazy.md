# Lazy Monad

`System.Lazy<T>` is a value that is computed the first time it is asked for and remembered after that. Funcky
does not add a lazy type; it adds the monad operations to the one the BCL already has, so lazy values can be
combined without forcing them.

## Creating lazy values

```cs
Lazy<T> Lazy.FromFunc<T>(Func<T> valueFactory)
Lazy<T> Lazy.Return<T>(T value)
```

`FromFunc` is `new Lazy<T>(factory)` with type inference. `Return` wraps a value that is already known,
which is what you need at the boundary where an eager value meets a lazy computation.

## Combining lazy values

```cs
Lazy<TResult> Select<T, TResult>(this Lazy<T> lazy, Func<T, TResult> selector)
Lazy<TResult> SelectMany<T, TResult>(this Lazy<T> lazy, Func<T, Lazy<TResult>> selector)
Lazy<TResult> SelectMany<T, TLazy, TResult>(this Lazy<T> lazy, Func<T, Lazy<TLazy>> selector, Func<T, TLazy, TResult> resultSelector)
Lazy<T> Flatten<T>(this Lazy<Lazy<T>> lazy)
```

Without these, combining two lazy values means reading `.Value` on both, which computes them, and wrapping the
result in a new `Lazy`, which defeats the purpose. With them, the combination is itself lazy, and nothing is
computed until someone reads the final `.Value`:

```cs
var config = Lazy.FromFunc(() => LoadConfiguration());         // expensive, maybe never needed
var connection = Lazy.FromFunc(() => OpenConnection());

var repository =
    from c in config
    from conn in connection
    select new Repository(c, conn);

// Nothing has been loaded or opened yet.

repository.Value;   // loads, opens, constructs; subsequent reads reuse the result
```

Only the lazy values that the final computation actually touches are evaluated. If `selector` in a
`SelectMany` never reads its argument, the inner lazy stays unevaluated.

## Lazy values over sequences

`Sequence` and `Traverse` on `IEnumerable<T>` turn a sequence of lazy values into a lazy sequence, and the
other monads have overloads for `Lazy<T>` too. Note that the resulting `Lazy<IEnumerable<T>>` is lazy in two
ways: the outer `Lazy` defers everything, and once forced, the inner sequence still evaluates each element's
lazy only as it is enumerated.

## Trimming and AOT

`Lazy<T>` has a constructor that creates `T` through its parameterless constructor by reflection, so the type
parameters of these methods carry `DynamicallyAccessedMembers` annotations to keep that constructor alive
under trimming. You do not have to do anything, but it explains the attribute in the signatures.
