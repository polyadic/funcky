# The TryVerb-pattern

The TryVerb pattern is used in several instances in C# as an alternative for functions which throw an exception. 


`Parse` throws an exception if `inputString` is not a number.

```cs
var number = int.Parse(inputString);
```   

`TryParse` returns `false` in such a case, so the number is always correct if `TryParse` returns `true`. This means you have to check the return value before accessing number.

	
```cs
if (int.TryParse(inputString, out number)) 
{
    // ...
}
```   

Out parameters are bad, and in consequence we think the TryVerb-pattern (`TryGet`, `TryParse`...) used in C# as an anti-pattern.


We have added an overload for each and every "Try" function we have found in the .NET Framework.


## TryGetValue

Extension functions have been added to `IDictionary` and `IReadOnlyDictionary`

## TryGetValues



## The parse functions

```cs
Option<int> = "1234".TryParseInt();
```   

