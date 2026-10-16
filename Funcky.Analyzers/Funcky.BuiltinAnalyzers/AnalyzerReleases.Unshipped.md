; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules

Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ0003 | Funcky.Correctness | Error | SyntaxSupportOnlyAnalyzer
λ1009 | Funcky.Correctness | Error | NonDefaultableAnalyzer, moved from Funcky.Analyzers 1.4 (keeps its ID)
λ1010 | Funcky.Correctness | Error | OptionListPatternAnalyzer, moved from Funcky.Analyzers before it shipped (keeps its ID)

### Removed Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ0002 | Funcky.Deprecation | Warning | OptionNoneMethodGroupAnalyzer

### Changed Rules

Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
--------|--------------|--------------|--------------|--------------|-------
λ0001 | Funcky.Correctness | Error | Funcky | Error | TryGetValueAnalyzer
