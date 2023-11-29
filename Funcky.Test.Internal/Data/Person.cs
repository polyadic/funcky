namespace Funcky.Test.Internal.Data;

public sealed class Person(int age) : IComparable<Person>, IEquatable<Person>
{
    public int Age { get; } = age;

    public int CompareTo(Person? other)
        => other != null
            ? Age.CompareTo(other.Age)
            : -1;

    public static Person Create(int age)
        => new(age);

    public static Person? Create(int? age)
        => age.HasValue
            ? new Person(age.Value)
            : null;

    public bool Equals(Person? other)
         => other?.Age == Age;

    public override bool Equals(object? other)
        => Equals(other as Person);

    public override int GetHashCode()
        => Age.GetHashCode();
}
