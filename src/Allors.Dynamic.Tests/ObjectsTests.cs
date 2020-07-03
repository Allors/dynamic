namespace Allors.Dynamic.Tests
{
    using System;
    using System.Linq;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class ObjectsTests
    {
        [Fact]
        public void Filter()
        {
            DynamicPopulation population = new DynamicPopulation(
                  new Pluralizer(),
                  v =>
            {
                v.AddUnit<Person, string>("FirstName");
                v.AddUnit<Person, string>("LastName");
            });

            dynamic Create<T>(params Action<T>[] builders)
                 where T : DynamicObject
            {
                return population.New<T>(builders);
            }

            Action<dynamic> FirstName(string firstName)
            {
                return (obj) => obj.FirstName = firstName;
            }

            Action<dynamic> LastName(string lastName)
            {
                return (obj) => obj.LastName = lastName;
            }

            dynamic NewPerson(string firstName, string lastName)
            {
                return Create<Person>(FirstName(firstName), LastName(lastName));
            }

            dynamic jane = NewPerson("Jane", "Doe");
            dynamic john = NewPerson("John", "Doe");
            dynamic jenny = NewPerson("Jenny", "Doe");

            dynamic[] lastNameDoe = population.Objects.Where(v => v.LastName == "Doe").ToArray();

            Assert.Equal(3, lastNameDoe.Length);
            Assert.Contains(jane, lastNameDoe);
            Assert.Contains(john, lastNameDoe);
            Assert.Contains(jenny, lastNameDoe);

            dynamic[] lessThanFourLetterFirstNames = population.Objects.Where(v => v.FirstName.Length < 4).ToArray();

            Assert.Empty(lessThanFourLetterFirstNames);

            dynamic[] fourLetterFirstNames = population.Objects.Where(v => v.FirstName.Length == 4).ToArray();

            Assert.Equal(2, fourLetterFirstNames.Length);
            Assert.Contains(jane, fourLetterFirstNames);
            Assert.Contains(john, fourLetterFirstNames);

            dynamic[] fiveLetterFirstNames = population.Objects.Where(v => v.FirstName.Length == 5).ToArray();
            Assert.Single(fiveLetterFirstNames);
            Assert.Contains(jenny, fiveLetterFirstNames);
        }
    }
}
