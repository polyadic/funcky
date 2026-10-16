# Introduction

Funcky is a functional programming library for C#. It gives you an `Option<T>` type for values that may be absent,
`Result<T>` and `Either<L, R>` for computations that may fail, a large set of extensions for `IEnumerable<T>` and `string`,
and a handful of small functional helpers. All of it is built on the parts of C# you already use: LINQ, lambdas,
and the type system.

## Who this book is for

You are a C# developer. You write LINQ queries without thinking about it, you know what `Select`, `Where` and `SelectMany` do,
and you are comfortable with lambdas. You have heard of functional programming, maybe of monads, but you have never
worked in a functional language and the terminology sounds more intimidating than it should.

This book takes that starting point seriously. It does not assume you know what a functor is. Where a functional
concept matters, the book explains it by pointing at something you already do in LINQ. You will find that you have
been using monads for years; you just called them `IEnumerable<T>`.

## The one idea behind Funcky

Funcky started from a blog post by Mark Seemann, [How to get the value out of the monad][seemann].
His observation was this: most `Maybe` or `Option` types come with a way to ask "is there a value?" and a way to
pull the value out. Programmers then use those two operations exclusively, and the type becomes a slightly more
verbose `null`.

> Unfortunately, Maybe implementations often come with an API that enables you to ask a Maybe object if it's populated
> or empty, and a way to extract the value from the Maybe container. This misleads many programmers into thinking that
> the proper way to use the Maybe type is to first check if the value is present, and then extract it.

Funcky takes the consequence. **There is no `Value` property, no `HasValue`, no `GetOrThrow`, and no `IsSome`.**
This is not an oversight, and it is not going to be added. Other libraries offer these members; Funcky deliberately does not.

If you come from `Nullable<T>` this will feel restrictive at first. You will hold an `Option<int>` and wonder how to
get the `int` out of it. That moment is the point of the library. The answer is almost never "get it out". The answer is
to tell the option what to do with the value if there is one, and let the option handle the case where there is none.
You already do this in LINQ: you do not pull elements out of an `IEnumerable<T>` one by one to transform them,
you call `Select`. `Option<T>` works the same way.

The book returns to this question in the [Option chapter](./option.md), which is organized as a ladder of answers
to "how do I get the value out?", from the ones you should reach for first to the ones that exist for interop with
imperative code.

## Why bother

Code written in this style has fewer places where it can go wrong. A missing value cannot be forgotten, because
the compiler will not let you use an `Option<int>` where an `int` is expected. A failed computation cannot be ignored,
because a `Result<T>` has to be unwrapped before its value is usable. Side effects become visible, because they
have to be placed explicitly rather than happening wherever a `null` check was missed.

None of this requires you to abandon the C# you know. Funcky's types support LINQ query syntax, work with pattern
matching, and slot into existing code one method at a time. The [case study on replacing `if null`](./case-studies/if-null-to-option.md)
shows a typical migration in four small steps.

## How to read this book

* If you want to understand the mindset first, continue with [Functional Programming](./fp/overview.md).
* If you want to start using the library, go to the [Option Monad](./option.md). It is the type you will use most,
  and everything else follows the same pattern.
* The chapters on `IEnumerable` and `string` extensions are reference material. Skim them once so you know what exists.
* The [analyzer rules](./analyzer-rules/analyzer-rules.md) are worth a look even if you never read them in full.
  The `Funcky.Analyzers` package flags code that reaches for the general tool when a specific one exists, and the
  explanation on each rule page doubles as a short lesson in idiomatic usage.

[seemann]: https://blog.ploeh.dk/2019/02/04/how-to-get-the-value-out-of-the-monad/
