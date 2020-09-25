# Simplify `if null` by using an `Option`

We start off with the following code:
(Note that some types have been omitted for brevity)
```csharp
#nullable enable

using System;

public class Example
{
    public VersionEnvironment? GetCurrentVersionEnvironment(PackageName packageName)
    {
        var currentVersion = ReadCurrentVersion(packageName);

        if (currentVersion is null)
        {
            return null;
        }

        var versionPath = GetVersionPath(packageName, currentVersion);
        return new VersionEnvironment(currentVersion, versionPath);
    }

    public PackageVersion? ReadCurrentVersion(PackageName name) => null; // Real implementation omitted

    public string GetVersionPath(PackageName name, PackageVersion version) => null!; // Real implementation omitted
}
```

The function `GetCurrentVersionEnvironment` doesn't do much, but it's not pleasant to look at,
because of that early return condition.

## Step Ⅰ

We start off by changing the return type of `ReadCurrentVersion` from `PackageVersion?` to an `Option`:
```csharp
public Option<PackageVersion> ReadCurrentVersion(PackageName name) => Option.None<PackageVersion>(); // Real implementation omitted
```

## Step Ⅱ
This immediately breaks the `GetCurrentVersionEnvironment`, because our `null` check no longer makes sense.
Instead of checking for `null` and returning early, we can use `Select` on the `Option` to project its value.

```csharp
public Option<VersionEnvironment> GetCurrentVersionEnvironment(PackageName packageName)
{
    return ReadCurrentVersion(packageName)
        .Select(currentVersion => {
            var versionPath = GetVersionPath(packageName, currentVersion);
            return new VersionEnvironment(currentVersion, versionPath);
        });
}
```

## Step Ⅲ
This is already much simpler, since we've got rid of that explicit null check.

There's still room for simplification, since our projection takes up two lines and we ideally only want a single expression in our projection.

We can achieve this by translating our expression to query syntax:

```csharp
public Option<VersionEnvironment> GetCurrentVersionEnvironment(PackageName packageName)
{
    return from currentVersion in ReadCurrentVersion(packageName)
           let versionPath = GetVersionPath(packageName, currentVersion)
           select new VersionEnvironment(currentVersion, versionPath);
}
```

## Step Ⅳ
Since our method now consists of only one expression, we can make it an expression body:
```csharp
public Option<VersionEnvironment> GetCurrentVersionEnvironment(PackageName packageName)
    => return from currentVersion in ReadCurrentVersion(packageName)
              let versionPath = GetVersionPath(packageName, currentVersion)
              select new VersionEnvironment(currentVersion, versionPath);
```