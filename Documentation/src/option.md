# Option Monad

`Option<T>` represents a value that may be absent. It is either `Some` with a value of type `T`, or `None`.
That is the whole type. Everything else in this chapter is about what you can do with it.

If you have used `Nullable<T>` or nullable reference types, the idea is familiar. The difference is in the API.
`Nullable<T>` lets you check `HasValue` and then read `Value`. `Option<T>` does not. Instead, you pass the option
a function describing what should happen with the value, and the option applies it only if a value exists.

## Think of it as a list with at most one element

The mental model that makes `Option<T>` click for LINQ users: **an `Option<T>` is a sequence with zero or one elements.**

You never write this with a list:

```cs
IEnumerable<int> numbers = GetNumbers();
var doubled = new List<int>();
if (numbers.Any())
{
    foreach (var n in numbers)
    {
        doubled.Add(n * 2);
    }
}
```

You write `numbers.Select(n => n * 2)`, and an empty input simply produces an empty output. `Option<T>` gives you the
same operations with the same names and the same semantics. `Select` on a `None` is a `None`. `Where` can turn a
`Some` into a `None`. `SelectMany` chains operations that each might produce nothing. Query syntax works.

Funcky even lets you take the model literally: `option.ToEnumerable()` gives you the zero-or-one-element sequence,
and list patterns match on it directly:

```cs
string Describe(Option<int> option)
    => option switch
    {
        [var value] => $"Some({value})",
        [] => "None",
    };
```

## Creating options

```cs
// Some
Option<int> some = Option.Some(42);
Option<int> alsoSome = 42;                  // implicit conversion from T
Option<int> viaReturn = Option.Return(42);  // same as Some, the name monads use

// None
Option<int> none = Option<int>.None;
Option<int> alsoNone = default;             // default is a valid None

// From a nullable
Option<string> fromNullable = Option.FromNullable(Environment.GetEnvironmentVariable("HOME"));

// From a condition
Option<int> even = Option.FromBoolean(number % 2 == 0, number);
```

The type parameter is constrained to `notnull`. An `Option<string?>` does not exist, because "absent" is what `None`
is for. `Option.FromNullable` is the bridge: it maps `null` to `None` and anything else to `Some`.

Most options you will work with do not come from these constructors. They come from the many `…OrNone` methods Funcky
adds to the BCL, which replace `null` returns, `-1` sentinels, exceptions and `Try…` out-parameters with an `Option<T>`:

```cs
Option<int> parsed = "42".ParseInt32OrNone();
Option<string> first = names.FirstOrNone();
Option<int> index = text.IndexOfOrNone(':');
Option<User> user = usersById.GetValueOrNone(id);
```

See [The TryVerb-pattern](./try-pattern.md) and [String Extensions](./string-extensions.md) for the full lists.

## How do I get the value out?

This is the question every newcomer asks, and the rest of this chapter is the answer. There is no `Value`
property and no `GetOrThrow`. The methods below are ordered from the ones you should reach for first to the
ones that exist only for interop with imperative code. When you find yourself further down the ladder than
necessary, the `Funcky.Analyzers` package will usually tell you.

### 1. Don't. Stay inside with `Select`

Most of the time you do not need the value out. You need to do something with it and are happy to get an option
back. That is `Select`, exactly as in LINQ:

```cs
Option<int> length = "42".ParseInt32OrNone().Select(n => n * 2);

// or in query syntax
Option<int> length =
    from n in "42".ParseInt32OrNone()
    select n * 2;
```

If the input is `None`, the lambda never runs and the result is `None`. No check, no branch, no way to forget the
empty case.

### 2. Chain operations that might each fail with `SelectMany`

When the function you want to apply itself returns an option, `Select` would give you an `Option<Option<T>>`.
`SelectMany` flattens that, again exactly as in LINQ:

```cs
Option<User> FindRequestingUser(IReadOnlyDictionary<string, string> headers)
    => from header in headers.GetValueOrNone("X-User-Id")
       from id in header.ParseInt32OrNone()
       from user in userRepository.FindOrNone(id)
       select user;
```

Three lookups, each of which can fail, and no `if` in sight. If any step yields `None`, the whole expression is `None`.
This is the operation that makes `Option<T>` a monad, and it is the reason the type exists.

### 3. Filter with `Where`

`Where` turns a `Some` into a `None` when the predicate is false:

```cs
Option<int> positive = "42".ParseInt32OrNone().Where(n => n > 0);
```

Again this composes with query syntax, so `from n in ... where n > 0 select n` reads exactly like it would over a list.

### 4. Provide a fallback with `GetOrElse` or `OrElse`

Eventually a value has to leave the option, usually at the boundary to code that does not use `Option<T>`.
The safe way to do that is to say what happens in the `None` case.

`GetOrElse` produces a plain `T` by supplying a fallback value:

```cs
int port = configuration.GetValueOrNone("port")
    .SelectMany(ParseExtensions.ParseInt32OrNone)
    .GetOrElse(8080);

// Lazy variant if the fallback is expensive to compute
string name = user.Select(u => u.Name).GetOrElse(() => LookupDefaultName());
```

`OrElse` stays inside the option and supplies an alternative option instead, which is what you want when you have
several places to look:

```cs
Option<string> setting = environmentVariable
    .OrElse(configurationFile)
    .OrElse(() => ReadFromRegistry());
```

### 5. Handle both cases with `Match`

`Match` is the most general way to leave an option. You provide a function for each case and get back whatever
those functions return:

```cs
string message = "42".ParseInt32OrNone().Match(
    none: "That was not a number",
    some: n => $"You entered {n}");
```

Every other method in this chapter could be written with `Match`, and that is exactly why you should treat it as
the last resort rather than the first. When one branch of a `Match` is the identity, you wanted `GetOrElse`.
When the `some` branch just wraps the value again, you wanted `OrElse` or `SelectMany`. The analyzers
[λ1005](./analyzer-rules/λ1005.md) to [λ1008](./analyzer-rules/λ1008.md) flag these cases and offer a code fix.

Always name the arguments (`none:`, `some:`). Both are functions of a similar shape and swapping them compiles.
[λ1003](./analyzer-rules/λ1003.md) enforces this.

### 6. Perform side effects with `Switch`, `AndThen` and `Inspect`

Functional code tries to push side effects to the edges, but eventually something has to be printed or written.

`Switch` is `Match` for actions; it returns nothing:

```cs
"42".ParseInt32OrNone().Switch(
    none: () => Console.WriteLine("not a number"),
    some: n => Console.WriteLine($"got {n}"));
```

`AndThen` runs an action only in the `Some` case:

```cs
userRepository.FindOrNone(id).AndThen(user => SendWelcomeMail(user));
```

`Inspect` and `InspectNone` run an action and hand the option back unchanged, which is useful for logging in the
middle of a chain, in the same way `Inspect` works on `IEnumerable<T>`:

```cs
Option<User> user = FindUser(id)
    .Inspect(u => logger.LogDebug("found {User}", u.Name))
    .InspectNone(() => logger.LogWarning("user {Id} not found", id));
```

### 7. Leave the world of options at the boundary

Sometimes the code on the other side speaks `null` or `IEnumerable<T>`, and you have to translate:

```cs
string? nullable = option.ToNullable();        // Some(x) -> x, None -> null
IEnumerable<int> items = option.ToEnumerable(); // Some(x) -> [x], None -> []
```

`ToNullable` is the right tool at an API boundary. Inside your own code prefer to keep the option;
[λ1008](./analyzer-rules/λ1008.md) will flag a `Match` that re-implements it.

### 8. `TryGetValue`, only where the language forces you

There is one escape hatch, `TryGetValue(out var value)`, and the built-in analyzer [λ0001](./analyzer-rules/λ0001.md)
reports an error if you use it anywhere except the two places where nothing else works: a loop condition and a
`catch … when` filter.

```cs
// A loop condition: you cannot write this with Select or Match.
var current = first;
while (current.TryGetValue(out var value))
{
    yield return value;
    current = successor(value);
}
```

If you find yourself wanting `TryGetValue` in ordinary code, go back up the ladder. There is almost always a
`Select`, `SelectMany` or `GetOrElse` that says the same thing without the check.

## Putting it together

Here is the shape of a typical function that uses options throughout. Notice that the only place a value leaves an
option is the last line, and that place states what happens when there is none:

```cs
string Greeting(IReadOnlyDictionary<string, string> headers)
    => (from header in headers.GetValueOrNone("X-User-Id")
        from id in header.ParseInt32OrNone()
        from user in userRepository.FindOrNone(id)
        where user.IsActive
        select $"Welcome back, {user.Name}")
        .GetOrElse("Welcome, stranger");
```

Compare that with the imperative version and count the `if` statements it would need. Every one of them is a place
where a `null` could slip through. Here, the compiler does not let it.

## Where to go next

* [The TryVerb-pattern](./try-pattern.md) lists the `…OrNone` methods that produce options from BCL calls.
* [Finding one element](./enumerable-extensions/finding-one-element.md) covers `FirstOrNone` and friends on
  sequences, and [`WhereSelect`](./enumerable-extensions/filtering-and-partitioning.md#whereselect) is how
  options and sequences combine.
* The [`if null` case study](./case-studies/if-null-to-option.md) walks through migrating an existing method.
* [`Result<T>`](./result.md) and [`Either<L, R>`](./either.md) follow the same ladder, with an error value in place of `None`.
