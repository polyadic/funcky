# Funcky.Xunit
### Stack Trace
All `FunctionalAssert` methods have `AggressiveInlining` set and re-throw the exceptions, because the stack trace should only contain the test method's
name (just like the `Assert` exceptions from xUnit).
