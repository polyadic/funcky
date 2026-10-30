; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### Removed Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1202 | Funcky.Style | Warning | PreferEnumerableSequenceConstructorsAnalyzer, the `Sequence` class it pointed away from is removed in Funcky 4.0.0
λ1203 | Funcky.Style | Warning | PreferXunitAssertAnalyzer, the `FunctionalAssert` class it pointed away from is removed in Funcky.Xunit 3.0.0 / Funcky.Xunit.v3 2.0.0
