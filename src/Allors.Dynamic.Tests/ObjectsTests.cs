namespace Allors.Dynamic.Tests
{
    using System;
    using System.Linq;
    using Xunit;

    public class ObjectsTests
    {
        [Fact]
        public void Filter()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                         .AddUnitRelationType("FirstName")
                         .AddUnitRelationType("LastName")
                      );

            dynamic Create(params Action<dynamic>[] builders)
            {
                return population.Create(builders);
            }

            Action<dynamic> FirstName(string firstName)
            {
                return (obj) => obj.FirstName = firstName;
            }

            Action<dynamic> LastName(string lastName)
            {
                return (obj) => obj.LastName = lastName;
            }

            dynamic person(string firstName, string lastName)
            {
                return Create(FirstName(firstName), LastName(lastName));
            }

            dynamic jane = person("Jane", "Doe");
            dynamic john = person("John", "Doe");
            dynamic jenny = person("Jenny", "Doe");

            dynamic[] does = population.Objects.Where(v => v.LastName == "Doe").ToArray();

            Assert.Equal(3, does.Length);
            Assert.Contains(jane, does);
            Assert.Contains(john, does);
            Assert.Contains(jenny, does);

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
