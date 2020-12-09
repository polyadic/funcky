using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Funcky.Test.Extensions.EnumerableExtensions
{
    internal class Person : IComparable<Person>
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
