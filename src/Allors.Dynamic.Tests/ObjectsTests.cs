using System;
using System.Linq;
using Allors.Dynamic.Meta;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class ObjectsTests
    {
        [Fact]
        public void Filter()
        {
            var meta = new DynamicMeta();
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(person, "FirstName");
            meta.AddUnit<string>(person, "LastName");

            var population = new DynamicPopulation(meta);

            dynamic NewPerson(string firstName, string lastName)
            {
                return population.New(person, v =>
                {
                    v.FirstName = firstName;
                    v.LastName = lastName;
                });
            }

            var jane = NewPerson("Jane", "Doe");
            var john = NewPerson("John", "Doe");
            var jenny = NewPerson("Jenny", "Doe");

            var lastNameDoe = population.Objects.Where(v => v.LastName == "Doe").ToArray();

            Assert.Equal(3, lastNameDoe.Length);
            Assert.Contains(jane, lastNameDoe);
            Assert.Contains(john, lastNameDoe);
            Assert.Contains(jenny, lastNameDoe);

            var lessThanFourLetterFirstNames = population.Objects.Where(v => v.FirstName.Length < 4).ToArray();

            Assert.Empty(lessThanFourLetterFirstNames);

            var fourLetterFirstNames = population.Objects.Where(v => v.FirstName.Length == 4).ToArray();

            Assert.Equal(2, fourLetterFirstNames.Length);
            Assert.Contains(jane, fourLetterFirstNames);
            Assert.Contains(john, fourLetterFirstNames);

            var fiveLetterFirstNames = population.Objects.Where(v => v.FirstName.Length == 5).ToArray();
            Assert.Single(fiveLetterFirstNames);
            Assert.Contains(jenny, fiveLetterFirstNames);
        }
    }
}
