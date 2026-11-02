# Funcky.Xunit
### Stack Trace
When an assertion of `AssertExtensions` (`Assert.Some`, `Assert.Ok`, …) fails, the stack trace of the exception should
start at the test method, just like the exceptions thrown by xUnit's own `Assert`. Every assertion therefore catches
the assertion exception and re-throws it, which resets the stack trace to the assertion method and removes the frames
of the monad and the lambda that actually threw.

On .NET 6 and later the methods are additionally marked with `[StackTraceHidden]`, so the assertion's own frame is
hidden as well and the trace starts at the test method.

On older frameworks (the `netstandard2.0` asset) there is no way to hide a frame, so the trace starts with one
`AssertExtensions` frame followed by the test method. `AggressiveInlining` does not help: the JIT never inlines a method
that contains an exception handler.
