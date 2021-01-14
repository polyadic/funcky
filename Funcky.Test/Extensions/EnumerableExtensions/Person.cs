using System;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    internal sealed class Person : IComparable<Person>
    {
        public Person(int age)
        {
            Age = age;
        }

        public int Age { get; }

        public int CompareTo(Person? other)
        {
            return other != null
                ? Age.CompareTo(other.Age)
                : -1;
        }
    }
}
