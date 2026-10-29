; Shipped analyzer releases
; https://github.com/dotnet/roslyn-analyzers/blob/main/src/Microsoft.CodeAnalysis.Analyzers/ReleaseTrackingAnalyzers.Help.md

## Release 1.0
### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1001 | Funcky | Warning | EnumerableRepeatOnceAnalyzer
λ1002 | Funcky | Warning | EnumerableRepeatNeverAnalyzer

## Release 1.1
### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1003 | Funcky | Warning | UseWithArgumentNamesAnalyzer
λ1004 | Funcky | Warning | JoinToStringEmptyAnalyzer

## Release 1.2
### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1005 | Funcky | Warning | OptionMatchAnalyzer
λ1006 | Funcky | Warning | OptionMatchAnalyzer
λ1007 | Funcky | Warning | OptionMatchAnalyzer
λ1008 | Funcky | Warning | OptionMatchAnalyzer

## Release 1.4
### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1009 | Funcky | Error | NonDefaultableAnalyzer

## Release 2.0
### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1011 | Funcky.Style | Warning | AlternativeMonadAnalyzer
λ1012 | Funcky.Style | Warning | PreferOrNoneAnalyzer
λ1013 | Funcky.Style | Info | SequenceReturnCollectionExpressionAnalyzer
λ1014 | Funcky.Style | Warning | ThrowInEagerFallbackAnalyzer
λ1015 | Funcky.Style | Info | EagerlyComputedFallbackAnalyzer
λ1016 | Funcky.Style | Warning | TrivialLazyFallbackAnalyzer
λ1017 | Funcky.Style | Warning | PreferNamedFunctionAnalyzer
λ1018 | Funcky.Style | Warning | PreferFlattenAnalyzer
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

## Release 2.1
### New Rules
Rule ID | Category | Severity | Notes
--------|----------|----------|-------
λ1201 | Funcky.Style | Warning | PreferStaticParseOrNoneAnalyzer
λ1202 | Funcky.Style | Warning | PreferEnumerableSequenceConstructorsAnalyzer
λ1203 | Funcky.Style | Warning | PreferXunitAssertAnalyzer
