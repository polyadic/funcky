using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Funcky.Monads;
using Funcky.Xunit;
using Xunit;

namespace Funcky.Test
{
    public sealed class JsonOptionConverterTest
    {
        [Fact]
        public void SerializesNoneAsNull()
        {
            const string expectedJson = "null";
            var json = JsonSerializer.Serialize(Option<string>.None());
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void SerializesNoneAsNullWhenNested()
        {
            const string expectedJson = @"{""BloodType"":""B-"",""EmergencyContact"":null}";
            var json = JsonSerializer.Serialize(new MedicalId(bloodType: "B-", emergencyContact: Option<Person>.None()));
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void SerializesInnerObjectWhenSomeIsGiven()
        {
            const string expectedJson = @"{""FirstName"":""Peter"",""LastName"":""Pan""}";
            var json = JsonSerializer.Serialize(Option.Some(new Person("Peter", "Pan")));
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void SerializesInnerObjectWhenNested()
        {
            const string expectedJson = @"{""BloodType"":""B-"",""EmergencyContact"":{""FirstName"":""Peter"",""LastName"":""Pan""}}";
            var @object = new MedicalId(bloodType: "B-", emergencyContact: new Person("Peter", "Pan"));
            var json = JsonSerializer.Serialize(@object);
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void SerializesInnerIntegerWhenSomeIsGiven()
        {
            const string expectedJson = @"42";
            var json = JsonSerializer.Serialize(Option.Some(42));
            Assert.Equal(expectedJson, json);
        }

        [Fact]
        public void DeserializesNoneFromNull()
        {
            const string json = @"null";
            FunctionalAssert.IsNone(JsonSerializer.Deserialize<Option<string>>(json));
        }

        [Fact]
        public void DeserializesSomeFromObject()
        {
            const string json = @"{""FirstName"":""Peter"",""LastName"":""Pan""}";
            var expectedObject = new Person("Peter", "Pan");
            FunctionalAssert.IsSome(expectedObject, JsonSerializer.Deserialize<Option<Person>>(json));
        }

        [Fact]
        public void DeserializesSomeFromNumber()
        {
            const string json = @"42";
            const int expectedInteger = 42;
            FunctionalAssert.IsSome(expectedInteger, JsonSerializer.Deserialize<Option<int>>(json));
        }

        [Fact]
        public void DeserializesNoneFromNullWhenNested()
        {
            const string json = @"{""BloodType"":""B-"",""EmergencyContact"":null}";
            var expectedObject = new MedicalId(bloodType: "B-", emergencyContact: Option<Person>.None());
            Assert.Equal(expectedObject, JsonSerializer.Deserialize<MedicalId>(json));
        }

        [Fact]
        public void DeserializesInnerObjectWhenNested()
        {
            const string json = @"{""BloodType"":""B-"",""EmergencyContact"":{""FirstName"":""Peter"",""LastName"":""Pan""}}";
            var expectedObject = new MedicalId(bloodType: "B-", emergencyContact: new Person("Peter", "Pan"));
            Assert.Equal(expectedObject, JsonSerializer.Deserialize<MedicalId>(json));
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201", Justification = "Records are not yet supported by StyleCop")]
        private sealed record Person
        {
            public Person(string firstName, string lastName)
            {
                FirstName = firstName;
                LastName = lastName;
            }

            public Person()
            {
            }

            public string FirstName { get; set; } = null!;

            public string LastName { get; set; } = null!;
        }

        [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1201", Justification = "Records are not yet supported by StyleCop")]
        private sealed record MedicalId
        {
            public MedicalId(string bloodType, Option<Person> emergencyContact)
            {
                BloodType = bloodType;
                EmergencyContact = emergencyContact;
            }

            public MedicalId()
            {
            }

            public string BloodType { get; set; } = null!;

            public Option<Person> EmergencyContact { get; set; }
        }
    }
}
