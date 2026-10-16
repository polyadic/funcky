# Reader Monad

`Reader<TEnvironment, TResult>` is a computation that needs an environment to run. It is a delegate, nothing
more:

```cs
public delegate TResult Reader<in TEnvironment, out TResult>(TEnvironment environment);
```

A reader is a function from the environment to a result. What makes it a monad is that readers compose: you
can build a bigger reader out of smaller ones with `Select` and `SelectMany`, and every one of them receives
the same environment when the whole thing is finally run. The environment is typically a configuration, a set
of services, or a connection, and the reader is the functional alternative to passing that object through
every method signature, or to a dependency injection container.

## Creating readers

```cs
Reader<TEnvironment, TResult> Reader<TEnvironment>.Return<TResult>(TResult value)       // ignores the environment
Reader<TEnvironment, TResult> Reader<TEnvironment>.FromFunc<TResult>(Func<TEnvironment, TResult> function)
Reader<TEnvironment, Unit> Reader<TEnvironment>.FromAction(Action<TEnvironment> action)
```

Because `Reader` is a delegate type, any lambda or method with the right shape converts to it, so most readers
are written directly:

```cs
record Configuration(char QuoteChar);

static Reader<Configuration, string> Quote(string text)
    => config => $"{config.QuoteChar}{text}{config.QuoteChar}";
```

`Reader<TEnvironment>.Return` lifts a plain value into a reader that does not look at the environment. The
split into a generic class with the environment and a generic method with the result exists so that `Return`
can be passed as a method group where the result type is inferred.

## Composing readers

`Select` transforms the result; `SelectMany` chains a reader with a function producing the next reader. Query
syntax works, and the environment is threaded through invisibly:

```cs
static Reader<Configuration, string> QuoteAll(string start, string middle, string end)
    => from s in Quote(start)
       from m in Quote(middle)
       from e in Quote(end)
       select $"{s}{m}{e}";
```

Nothing has run yet. `QuoteAll` returns a reader, and the environment is supplied once, at the edge:

```cs
var reader = QuoteAll("a", "b", "c");

reader(new Configuration('"'));   // "\"a\"\"b\"\"c\""
reader(new Configuration('*'));   // "*a**b**c*"
```

The same reader run with two configurations gives two results. That is the point: the code that builds the
computation does not know or care which environment it will see.

## Readers over sequences

`Sequence` and `Traverse` on `IEnumerable<T>` work with readers: a sequence of readers becomes a reader of a
sequence, run lazily under one environment.

```cs
static Reader<Configuration, IEnumerable<string>> QuoteEach(IEnumerable<string> texts)
    => texts.Traverse(Quote);
```

The other monads have `Traverse` and `Sequence` overloads for readers as well, for example
`Option<Reader<E, T>>` to `Reader<E, Option<T>>`.

## Side effects

`FromAction` wraps an `Action<TEnvironment>` into a `Reader<TEnvironment, Unit>`, so an effectful step can take
part in a query. The action runs when the reader is run, not when it is created.

```cs
static Reader<Configuration, Unit> Log(string message)
    => Reader<Configuration>.FromAction(config => Console.WriteLine($"[{config.QuoteChar}] {message}"));
```
