using Xunit;

namespace Allors.Dynamic.Tests
{
    public class ManyToManyTests
    {
        [Fact]
        public void Add()
        {
            var population = new DynamicPopulation(v => v
                 .AddOneToManyAssociation("Employer", "Employee")
              );

            dynamic acme = population.Create();
            dynamic hooli = population.Create();

            dynamic jane = population.Create();
            dynamic john = population.Create();
            dynamic jenny = population.Create();

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
