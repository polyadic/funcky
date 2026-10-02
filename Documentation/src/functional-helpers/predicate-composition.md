# Predicate Composition

Builds a new predicate from existing ones, so that `Where`, `Any`, `All` and friends can be given a composed
condition without a lambda that restates it.

```cs
Func<T, bool> All<T>(params Func<T, bool>[] predicates)
Func<T, bool> Any<T>(params Func<T, bool>[] predicates)
Func<T, bool> Not<T>(Func<T, bool> predicate)
```

`All` is true when every predicate is true, `Any` when at least one is, `Not` inverts. They short-circuit like
`&&` and `||`, and the predicates are evaluated left to right. `All()` with no predicates is always true and
`Any()` with none always false, following the LINQ methods of the same names.

The point is reuse. Predicates that have names can be combined by name, where a lambda would have to spell
out each one again:

```cs
Func<User, bool> isActive = u => u.IsActive;
Func<User, bool> isAdmin = u => u.Role == Role.Admin;
Func<User, bool> isVerified = u => u.EmailVerified;

var activeAdmins = users.Where(All(isActive, isAdmin));
var needsAttention = users.Where(Any(Not(isVerified), Not(isActive)));

// Compared to
var activeAdmins = users.Where(u => isActive(u) && isAdmin(u));
```

Because the result is an ordinary `Func<T, bool>`, it composes further: `Not(All(a, b))` is "not both".

## True and False

`True` and `False` are predicates that ignore their arguments and return a constant, with overloads for zero
to four parameters:

```cs
bool True()
bool True<T1>(T1 ω1)
bool True<T1, T2>(T1 ω1, T2 ω2)
// … and the same for False
```

They are the predicate equivalents of [`Identity`](./identity.md) and [`NoOperation`](./no-operation.md):
the thing to pass when an API demands a predicate and you have no condition. `SingleOrNone()` is implemented
as `SingleOrNone(True)`.

```cs
// A feature flag that is always on in tests
Func<Request, bool> filter = isTestEnvironment ? True : IsAllowed;

// A default for an optional predicate parameter
IEnumerable<T> Filter<T>(IEnumerable<T> items, Func<T, bool>? predicate = null)
    => items.Where(predicate ?? True);
```
