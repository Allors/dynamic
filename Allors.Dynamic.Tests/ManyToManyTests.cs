using Xunit;

namespace Allors.Dynamic.Tests
{
    public class ManyToManyTests
    {
        [Fact]
        public void SingleActiveLink()
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

            Assert.Single(jane.Employers);
            Assert.Contains(acme, jane.Employers);

            Assert.Single(john.Employers);
            Assert.Contains(acme, john.Employers);

            Assert.Single(jenny.Employers);
            Assert.Contains(acme, jenny.Employers);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Empty(hooli.Employees);
        }

        [Fact]
        public void MultipeleActiveLinks()
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

            hooli.AddEmployee(jane);

            Assert.Equal(2, jane.Employers.Length);
            Assert.Contains(acme, jane.Employers);
            Assert.Contains(hooli, jane.Employers);

            Assert.Single(john.Employers);
            Assert.Contains(acme, john.Employers);

            Assert.Single(jenny.Employers);
            Assert.Contains(acme, jenny.Employers);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Single(hooli.Employees);
            Assert.Contains(jane, hooli.Employees);

            hooli.AddEmployee(john);

            Assert.Equal(2, jane.Employers.Length);
            Assert.Contains(acme, jane.Employers);
            Assert.Contains(hooli, jane.Employers);

            Assert.Equal(2, john.Employers.Length);
            Assert.Contains(acme, john.Employers);
            Assert.Contains(hooli, john.Employers);

            Assert.Single(jenny.Employers);
            Assert.Contains(acme, jenny.Employers);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(2, hooli.Employees.Length);
            Assert.Contains(jane, hooli.Employees);
            Assert.Contains(john, hooli.Employees);

            hooli.AddEmployee(jenny);

            Assert.Equal(2, jane.Employers.Length);
            Assert.Contains(acme, jane.Employers);
            Assert.Contains(hooli, jane.Employers);

            Assert.Equal(2, john.Employers.Length);
            Assert.Contains(acme, john.Employers);
            Assert.Contains(hooli, john.Employers);

            Assert.Equal(2, jenny.Employers.Length);
            Assert.Contains(acme, jenny.Employers);
            Assert.Contains(hooli, jenny.Employers);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(3, hooli.Employees.Length);
            Assert.Contains(jane, hooli.Employees);
            Assert.Contains(john, hooli.Employees);
            Assert.Contains(jenny, hooli.Employees);
        }
    }
}