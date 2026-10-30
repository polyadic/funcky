# Support Policy

Funcky is a stable base-library which tries to support many scenario's in the .NET world.

## What Frameworks do we support?

We support the all the actively supported modern Frameworks, .NET Standard 2.1, 2.0 And.NET 4.8+

* We only remove support for target frameworks on major version changes, to avoid breaking changes for the consumers.

## What SDKs do we support?

The analyzers built into Funcky and the Funcky.Analyzers package are compiled against a fixed Roslyn version. Funcky 4
targets Roslyn 4.14, the compiler of Visual Studio 2022 17.14 and the .NET SDK 9.0.300, so every .NET 10 SDK and newer
loads them. Older SDKs skip the analyzers; the library itself has no SDK requirement beyond its target frameworks.

* We only raise the Roslyn baseline on major version changes.

## Testing

Testing with old frameworks which are not supported anymore is getting painful, we therefore only test with frameworks which are still in active support.

## Next removal

Funcky 4 will remove the following target frameworks:

* net7.0
* net6.0
* net5.0
* netcoreapp3.1
