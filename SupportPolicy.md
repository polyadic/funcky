# Support Policy

Funcky is a stable base-library which tries to support many scenario's in the .NET world.

## What Frameworks do we support?

We support all the actively supported modern Frameworks, .NET Standard 2.1, 2.0 and .NET 4.8+

* We only remove support for target frameworks on major version changes, to avoid breaking changes for the consumers.

## Testing

Testing with old frameworks which are not supported anymore is getting painful, we therefore only test with frameworks which are still in active support.

## Removed target frameworks

Funcky 4.0.0 removed these target frameworks (all out of support at the time):

* net7.0
* net6.0
* net5.0
* netcoreapp3.1

The oldest .NET (Core) asset is now `net8.0`; .NET Framework consumers keep using the `netstandard2.0` asset.

## Next removal

Funcky 5 will remove the target frameworks that have left Microsoft's support by then. Candidates today:

* net8.0 (support ends 10 November 2026)
* net9.0 (support ended 12 May 2026)
