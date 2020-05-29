namespace Allors.Dynamic.Tests
{
    using System;
    using Xunit;

    public class BuilderTests
    {
        [Fact]
        public void Set()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                .AddUnitRelationType("Name")
                .AddOneToOneRelationType("Property", "Owner")
             );

            dynamic Create(params Action<dynamic>[] builders)
            {
                return population.Create(builders);
            }

            Action<dynamic> Name(string name)
            {
                return (obj) => obj.Name = name;
            }

            Action<dynamic> Owner(dynamic owner)
            {
                return (obj) => obj.Owner = owner;
            }

            dynamic acme = Create(Name("Acme"), Owner(Create(Name("Jane"))));

            Assert.Equal("Acme", acme.Name);

            dynamic jane = acme.Owner;
            Assert.Equal("Jane", jane.Name);
        }
    }
}
