namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class ManyToManyTests
    {
        [Fact]
        public void SingleActiveLink()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                 .AddManyToMany<Organisation, Person>("Employer", "Employee"));

            dynamic acme = population.New<Organisation>();
            dynamic hooli = population.New<Organisation>();

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

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
            DynamicPopulation population = new DynamicPopulation(v => v
                 .AddManyToMany<Organisation, Person>("Employer", "Employee"));

            dynamic acme = population.New<Organisation>();
            dynamic hooli = population.New<Organisation>();

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

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