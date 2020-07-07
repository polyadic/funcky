# Funcky.Xunit

## Exceptions

### Message
The exceptions need to be in the "Xunit.Sdk" namespace, because the error messages would otherwise contain the exception's name.
See: [xunit/src/common/ExceptionUtility.cs](https://github.com/xunit/xunit/blob/a89b441170f0fe76e510cc2b1839a5023ded4f99/src/common/ExceptionUtility.cs#L89)

### Stack Trace
All `FunctionalAssert` methods have `AggressiveInlining` set and re-throw the exceptions, because the stack trace should only contain the test method's
name (just like the `Assert` exceptions from xUnit).
