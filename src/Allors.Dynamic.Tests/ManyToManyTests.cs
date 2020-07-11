namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Meta;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class ManyToManyTests
    {
        [Fact]
        public void SingleActiveLink()
        {
            var population = new Default.DynamicPopulation(
                  new DynamicMeta(new Pluralizer()),
                  v => v.AddManyToMany<Organisation, Person>("Employee"));

            dynamic acme = population.New<Organisation>();
            dynamic hooli = population.New<Organisation>();

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            Assert.Single(jane.OrganisationWhereEmployee);
            Assert.Contains(acme, jane.OrganisationWhereEmployee);

            Assert.Single(john.OrganisationWhereEmployee);
            Assert.Contains(acme, john.OrganisationWhereEmployee);

            Assert.Single(jenny.OrganisationWhereEmployee);
            Assert.Contains(acme, jenny.OrganisationWhereEmployee);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Empty(hooli.Employees);
        }

        [Fact]
        public void MultipeleActiveLinks()
        {
            var population = new Default.DynamicPopulation(
                  new DynamicMeta(new Pluralizer()),
                   v => v.AddManyToMany<Organisation, Person>("Employee"));

            dynamic acme = population.New<Organisation>();
            dynamic hooli = population.New<Organisation>();

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            hooli.AddEmployee(jane);

            Assert.Equal(2, jane.OrganisationWhereEmployee.Length);
            Assert.Contains(acme, jane.OrganisationWhereEmployee);
            Assert.Contains(hooli, jane.OrganisationWhereEmployee);

            Assert.Single(john.OrganisationWhereEmployee);
            Assert.Contains(acme, john.OrganisationWhereEmployee);

            Assert.Single(jenny.OrganisationWhereEmployee);
            Assert.Contains(acme, jenny.OrganisationWhereEmployee);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Single(hooli.Employees);
            Assert.Contains(jane, hooli.Employees);

            hooli.AddEmployee(john);

            Assert.Equal(2, jane.OrganisationWhereEmployee.Length);
            Assert.Contains(acme, jane.OrganisationWhereEmployee);
            Assert.Contains(hooli, jane.OrganisationWhereEmployee);

            Assert.Equal(2, john.OrganisationWhereEmployee.Length);
            Assert.Contains(acme, john.OrganisationWhereEmployee);
            Assert.Contains(hooli, john.OrganisationWhereEmployee);

            Assert.Single(jenny.OrganisationWhereEmployee);
            Assert.Contains(acme, jenny.OrganisationWhereEmployee);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(2, hooli.Employees.Length);
            Assert.Contains(jane, hooli.Employees);
            Assert.Contains(john, hooli.Employees);

            hooli.AddEmployee(jenny);

            Assert.Equal(2, jane.OrganisationWhereEmployee.Length);
            Assert.Contains(acme, jane.OrganisationWhereEmployee);
            Assert.Contains(hooli, jane.OrganisationWhereEmployee);

            Assert.Equal(2, john.OrganisationWhereEmployee.Length);
            Assert.Contains(acme, john.OrganisationWhereEmployee);
            Assert.Contains(hooli, john.OrganisationWhereEmployee);

            Assert.Equal(2, jenny.OrganisationWhereEmployee.Length);
            Assert.Contains(acme, jenny.OrganisationWhereEmployee);
            Assert.Contains(hooli, jenny.OrganisationWhereEmployee);

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