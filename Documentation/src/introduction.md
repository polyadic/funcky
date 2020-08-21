Functional programming is the oldest of the three major programming paradigms, none the less it is the last which gets wide spread usage. Even in languages like C++, Java or C# we want to use a functional style of programming.

Linq is the first Monad which got wide spread use in C#, and most C# programmers were not even aware of it beeing a monad, which probably helped.

[Mark Seemann] points out that "Unfortunately, Maybe implementations often come with an API that enables you to ask a Maybe object if it's populated or empty, and a way to extract the value from the Maybe container. This misleads many programmers \[...]"

This library is based on his example code, and should grow slowly to a library which helps to use and understand the Functional programming paradigm. Functional programming is side-effect free and the strong type system can be used to make illegal state impossible.

Use functional programming as an additional asset to write correct code.

[Mark Seemann]: https://blog.ploeh.dk/2019/02/04/how-to-get-the-value-out-of-the-monad/
