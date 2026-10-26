# Nullable Member Extensions
A handful of base class library members return `null` as a regular outcome rather than as an error: a type without a
base type, an assembly without an entry point, a directory at the root of its drive. Funcky offers each of them as an
`Option` through a C# 14 extension property, so the absence is a `None` rather than a `null` check.

```cs
Option<Type> baseType = typeof(Foo).BaseTypeOrNone;                 // None for object, interfaces and pointer types
Option<Type> declaringType = typeof(Foo).DeclaringTypeOrNone;       // None for a type that is not nested
Option<MethodInfo> entryPoint = assembly.EntryPointOrNone;          // None for a class library
Option<Exception> cause = exception.InnerExceptionOrNone;           // None without an inner exception
Option<DirectoryInfo> parent = directory.ParentOrNone;              // None for a root directory
Option<Uri> location = response.Headers.LocationOrNone;             // None without a Location header
```

| Property | On | Wraps |
|----------|----|-------|
| `BaseTypeOrNone` | `Type` | `Type.BaseType` |
| `DeclaringTypeOrNone` | `MemberInfo` (and therefore `Type`) | `MemberInfo.DeclaringType` |
| `EntryPointOrNone` | `Assembly` | `Assembly.EntryPoint` |
| `InnerExceptionOrNone` | `Exception` | `Exception.InnerException` |
| `ParentOrNone` | `DirectoryInfo` | `DirectoryInfo.Parent` |
| `LocationOrNone` | `HttpResponseHeaders` | `HttpResponseHeaders.Location` |

The properties need the C# 14 compiler. On an older compiler they are available as the static methods the compiler
lowers them to, `TypeExtensions.get_BaseTypeOrNone(type)`, which is rarely worth it over `Option.FromNullable(type.BaseType)`.
Members that return `null` from a method call, such as `Environment.GetEnvironmentVariable`, are out of scope here; wrap
them with [`Option.FromNullable`](./option.md#creating-options).
