using Xunit;

namespace Allors.Dynamic.Tests
{
    public class CompositeManyToManyTests
    {
        [Fact]
        public void Add()
        {
            var population = new DynamicPopulation(v => v
                 .AddOneToManyRelation("Employer", "Employee")
              );

            dynamic acme = population.NewObject();
            dynamic hooli = population.NewObject();

            dynamic jane = population.NewObject();
            dynamic john = population.NewObject();
            dynamic jenny = population.NewObject();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.DoesNotContain(jane, hooli.Employees);
            Assert.DoesNotContain(john, hooli.Employees);
            Assert.DoesNotContain(jenny, hooli.Employees);
        }
    }
}
