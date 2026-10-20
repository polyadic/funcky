; Unshipped analyzer release
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1011 | Funcky.Style | Warning | AlternativeMonadAnalyzer
λ1013 | Funcky.Style | Info | SequenceReturnCollectionExpressionAnalyzer
λ1014 | Funcky.Style | Warning | ThrowInEagerFallbackAnalyzer
λ1015 | Funcky.Style | Info | EagerlyComputedFallbackAnalyzer
λ1016 | Funcky.Style | Warning | TrivialLazyFallbackAnalyzer
λ1101 | Funcky.Style | Warning | FunctionalAssertAnalyzer

### Removed Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1009 | Funcky | Error | NonDefaultableAnalyzer, moved into the built-in analyzers of the Funcky package (keeps its ID)

### Changed Rules

Rule ID | New Category | New Severity | Old Category | Old Severity | Notes
--------|--------------|--------------|--------------|--------------|-------
λ1001 | Funcky.Style | Warning | Funcky | Warning | EnumerableRepeatOnceAnalyzer
λ1002 | Funcky.Style | Warning | Funcky | Warning | EnumerableRepeatNeverAnalyzer
λ1003 | Funcky.Style | Warning | Funcky | Warning | UseWithArgumentNamesAnalyzer
λ1004 | Funcky.Style | Warning | Funcky | Warning | JoinToStringEmptyAnalyzer
λ1005 | Funcky.Style | Warning | Funcky | Warning | OptionMatchAnalyzer
λ1006 | Funcky.Style | Warning | Funcky | Warning | OptionMatchAnalyzer
λ1007 | Funcky.Style | Warning | Funcky | Warning | OptionMatchAnalyzer
λ1008 | Funcky.Style | Warning | Funcky | Warning | OptionMatchAnalyzer
