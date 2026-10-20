Analyzers and code fixes that guide you towards idiomatic usage of [Funcky].

```xml
<PackageReference Include="Funcky.Analyzers" Version="..." PrivateAssets="all" />
```

The attributes behind the rules λ1003 (`[UseWithArgumentNames]`) and λ1009 (`[NonDefaultable]`) can be generated into your own
project, so that the rules also apply to your own members:

```xml
<PropertyGroup>
  <FunckyCodeAnalysisAttributes>UseWithArgumentNames;NonDefaultable</FunckyCodeAnalysisAttributes>
</PropertyGroup>
```

[Funcky]: https://www.nuget.org/packages/Funcky
