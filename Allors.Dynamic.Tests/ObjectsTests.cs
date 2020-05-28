using System;
using System.Linq;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class ObjectsTests
    {
        [Fact]
        public void Filter()
        {
            var population = new DynamicPopulation(v => v
                         .AddDataAssociation("FirstName")
                         .AddDataAssociation("LastName")
                      );

            dynamic Create(params Action<dynamic>[] builders) => population.Create(builders);
            Action<dynamic> FirstName(string firstName) => (obj) => obj.FirstName = firstName;
            Action<dynamic> LastName(string lastName) => (obj) => obj.LastName = lastName;

            dynamic person(string firstName, string lastName) => Create(FirstName(firstName), LastName(lastName));

            var jane = person("Jane", "Doe");
            var john = person("John", "Doe");
            var jenny = person("Jenny", "Doe");

            var does = population.Objects.Where(v => v.LastName == "Doe").ToArray();

            Assert.Equal(3, does.Length);
            Assert.Contains(jane, does);
            Assert.Contains(john, does);
            Assert.Contains(jenny, does);

            var fourLetterFirstNames = population.Objects.Where(v => v.FirstName.Length == 4).ToArray();

            Assert.Equal(2, fourLetterFirstNames.Length);
            Assert.Contains(jane, fourLetterFirstNames);
            Assert.Contains(john, fourLetterFirstNames);
        }
    }
}
