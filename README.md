# Funcky

Funcky is a functional Library for C# which encourages correct usage of the functional programming paradigm.

[![Build](https://github.com/messerli-informatik-ag/funcky/workflows/Build/badge.svg)](https://github.com/messerli-informatik-ag/funcky/actions?query=workflow%3ABuild)
[![NuGet package](https://buildstats.info/nuget/Funcky)](https://www.nuget.org/packages/Funcky)

Functional programming is the oldest of the three major programming paradigms, none the less it is the last which gets wide spread usage. Even in languages like C++, Java or C# we want to functional style of programming.

Linq is the first Monad which got wide spread use in C#, and most C# programmers were not even aware of it beeing a monad, which probably helped.

Mark Seemann points out that "Unfortunately, Maybe implementations often come with an API that enables you to ask a Maybe object if it's populated or empty, and a way to extract the value from the Maybe container. This misleads many programmers [...]"

https://blog.ploeh.dk/2019/02/04/how-to-get-the-value-out-of-the-monad/

This library is based on his example code, and should grow slowly to a library which helps to use and understand the Functional programming paradigm. Functional programming is side-effect free and the strong type system can be used to make illegal state impossible. 

Use functional programming as an additional asset to write correct code.

## Option Monad

An Option<T> can either hold a value of T (Some) or it holds Nothing (None)

### Create something

```csharp
var something = Option.Some(1337);
```
    
### Create nothing

```csharp
var nothing = Option<int>.None();
```

### Select

```csharp
Option<bool> maybeBool =
    from m in maybe
    select m == 1337;
```

### Select Many 

```csharp
var result = from number in someNumber
    from date in someDate
    select Tuple.Create(number, date);
```

### Match

```csharp
bool isSome = maybe.Match(
    none: false,
    some: m => true
);
```
