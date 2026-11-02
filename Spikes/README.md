# Spikes

Time-boxed prototypes that are **not meant to be merged**. They live in their own solution so that the main build and the
packages stay untouched; the branch name `spike/...` makes the nightly workflow publish the Funcky packages of the branch
under a `spike-<branch>.<date>.<run>` label.

```
dotnet test Spikes/Funcky.Spikes.slnx -c Release
dotnet run --project Spikes/Funcky.Spike.Benchmarks -c Release -- --filter '*'
```

Always Release: the JIT does not optimize debuggable assemblies, so against a Debug build of Funcky the allocation
tests are off by one boxed object per Option (the `item is null` check in the constructor).

## #1034 Option, Either and Result on C# unions and closed hierarchies

`Funcky.Spike/Unions` holds three shapes of `Option<T>` next to today's struct:

| Shape | Declaration | Where the value lives |
|---|---|---|
| today | `readonly struct Option<T>` with `bool` + `T` | inline, no allocation |
| `UnionOption<T>` | `union UnionOption<T>(Some<T>, None)` | one `object` field, the case struct is boxed |
| `BareOption<T>` | `union BareOption<T>(T, None)` | one `object` field, `T` boxed if a struct |
| `ClosedOption<T>` | `closed record ClosedOption<T>` with nested `Some`/`None` records | heap object per Some, singleton None |
| `CustomUnionOption<T>` | today's struct + `[Union]` + `IUnion` (`object Value`) | inline like today; the compiler treats it as a union |

plus `UnionEither<L, R>` and `UnionResult<T>` (with `Exception` as a bare case). The facts the write-up on #1034 rests
on are assertions in `Funcky.Spike.Test/ShapeFactsTest.cs`, `UnionOptionTest.cs` and `ClosedOptionTest.cs`;
the running write-up with the benchmark tables is [FINDINGS.md](FINDINGS.md).

## #862 Type traits (higher-kinded types via static abstract members)

`Funcky.Spike/Traits` is the lang-ext scheme: `K<F, A>` plus `Functor`/`Applicative`/`Monad`/`Traversable`/`Alternative`
traits whose members are static abstract on a marker type. Instances exist for today's struct `Option` and `Either`
(wrapped), for the union and closed shapes, and for `IEnumerable`. `Generic.cs` has Traverse, Sequence, Lift and Kleisli
composition written once. `TraitsTest.cs` runs the same generic program on every Option shape and measures the boxing.
