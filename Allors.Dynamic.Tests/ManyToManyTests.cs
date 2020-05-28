using Xunit;

namespace Allors.Dynamic.Tests
{
    public class ManyToManyTests
    {
        [Fact]
        public void AddSameAssociation()
        {
            var population = new DynamicPopulation(v => v
                 .AddManyToManyAssociation("Employer", "Employee"));

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

        [Fact]
        public void AddDifferentAssociation()
        {
            var population = new DynamicPopulation(v => v
                 .AddDataAssociation("FirstName")
                 .AddDataAssociation("LastName")
                 .AddManyToManyAssociation("Employer", "Employee"));

            dynamic acme = population.Create();
            dynamic hooli = population.Create();

            dynamic jane = population.Create();
            jane.FirstName = "Jane";
            jane.LastName = "Doe";
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

            Assert.Equal(jane.FirstName, "Jane");
            Assert.Equal(jane.LastName, "Doe");
        }
    }
}
