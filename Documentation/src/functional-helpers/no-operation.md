The `NoOperation` function is a more expressive way of manually creating an empty statement as a parameter to a method expecting a `Action`/`Action<T>`, supporting from 0 up to 8 generic parameters.

Example 1:

```
// The function we want to call:
public void DoSomething(int value, Action<int> callback)
{
  // ...
}

// How we would usually call it when we don't need the callback:
DoSomething(2, _ => {});

// How you can call it with NoOperation:
DoSomething(2, NoOperation);

```

`NoOperation` becomes especially useful when a `Action<T>` with many parameters is expected.

Example 2:

```
// The function we want to call:
public void DoSomething(int value, Action<int, string, float, AnyCustomClassThatYouWant> callback)
{
  // ...
}

// How we would usually call it when we don't need the callback (C#9):
DoSomething(2, (_, _, _, _) => {});
// Before C#9, this was even worse:
DoSomething(3, (_, __, ___, ____) => {});

// How you can call it with NoOperation:
DoSomething(2, NoOperation);
```

`NoOperation` is also useful when you want to use a expression body for a method.

Example 3:
```
// Abstract class:
public abstract class SomeClassWithExecuteAndHookBase
{
  public void Execute();
  
  protected abstract void PostExecutionHook();
}

// Derived class that doesn't have any use for the PostExecutionHook usually:
public class SomeClassWithExecuteAndHookDerived : SomeClassWithExecuteAndHookBase
{
  protected override void PostExecutionHook()
  {
  }
}

// Derived class that doesn't have any use for the PostExecutionHook with NoOperation:
public class SomeClassWithExecuteAndHookDerived : SomeClassWithExecuteAndHookBase
{
  protected override void PostExecutionHook() => NoOperation();
}
```