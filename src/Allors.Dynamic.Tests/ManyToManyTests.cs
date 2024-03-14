namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Meta;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class ManyToManyTests
    {
        [Fact]
        public void AddSingleActiveLink()
        {
            var population = new DynamicPopulation(
                  new DynamicMeta(),
                  v => v.AddUnit<Organization, string>("Name"),
                  v => v.AddManyToMany<Organization, Person>("Employee"));

            dynamic acme = population.New<Organization>(v => v.Name("Acme"));
            dynamic hooli = population.New<Organization>(v => v.Name("Hooli"));

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Single(jenny.OrganizationWhereEmployee);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Empty(hooli.Employees);
        }

        [Fact]
        public void SetSingleActiveLink()
        {
            var population = new DynamicPopulation(
                new DynamicMeta(),
                v => v.AddUnit<Organization, string>("Name"),
                v => v.AddManyToMany<Organization, Person>("Employee"));

            dynamic acme = population.New<Organization>(v => v.Name("Acme"));
            dynamic hooli = population.New<Organization>(v => v.Name("Hooli"));

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

            acme.Employees = new[] { jane };

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Empty(john.OrganizationWhereEmployee);

            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Equal(1, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.Employees = new[] { jane, john };

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Equal(2, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.Employees = new[] { jane, john, jenny };

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Single(jenny.OrganizationWhereEmployee);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.Employees = new Person[] { };

            Assert.Empty(jane.OrganizationWhereEmployee);
            Assert.Empty(john.OrganizationWhereEmployee);
            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Empty(acme.Employees);
            Assert.Empty(hooli.Employees);
        }

        [Fact]
        public void RemoveSingleActiveLink()
        {
            var population = new DynamicPopulation(
                new DynamicMeta(),
                v => v.AddUnit<Organization, string>("Name"),
                v => v.AddManyToMany<Organization, Person>("Employee"));

            dynamic acme = population.New<Organization>(v => v.Name("Acme"));
            dynamic hooli = population.New<Organization>(v => v.Name("Hooli"));

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

            acme.Employees = new[] { jane, john, jenny };

            acme.RemoveEmployee(jenny);

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Equal(2, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.RemoveEmployee(john);

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Empty(john.OrganizationWhereEmployee);

            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Equal(1, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.RemoveEmployee(jane);

            Assert.Empty(jane.OrganizationWhereEmployee);
            Assert.Empty(john.OrganizationWhereEmployee);
            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Empty(acme.Employees);
            Assert.Empty(hooli.Employees);
        }

        [Fact]
        public void MultipeleActiveLinks()
        {
            var population = new DynamicPopulation(
                  new DynamicMeta(),
                  v => v.AddUnit<Organization, string>("Name"),
                  v => v.AddManyToMany<Organization, Person>("Employee"));

            dynamic acme = population.New<Organization>(v => v.Name("Acme"));
            dynamic hooli = population.New<Organization>(v => v.Name("Hooli"));

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            hooli.AddEmployee(jane);

            Assert.Equal(2, jane.OrganizationWhereEmployee.Length);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);
            Assert.Contains(hooli, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Single(jenny.OrganizationWhereEmployee);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Single(hooli.Employees);
            Assert.Contains(jane, hooli.Employees);

            hooli.AddEmployee(john);

            Assert.Equal(2, jane.OrganizationWhereEmployee.Length);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);
            Assert.Contains(hooli, jane.OrganizationWhereEmployee);

            Assert.Equal(2, john.OrganizationWhereEmployee.Length);
            Assert.Contains(acme, john.OrganizationWhereEmployee);
            Assert.Contains(hooli, john.OrganizationWhereEmployee);

            Assert.Single(jenny.OrganizationWhereEmployee);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Length);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(2, hooli.Employees.Length);
            Assert.Contains(jane, hooli.Employees);
            Assert.Contains(john, hooli.Employees);

            hooli.AddEmployee(jenny);

            Assert.Equal(2, jane.OrganizationWhereEmployee.Length);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);
            Assert.Contains(hooli, jane.OrganizationWhereEmployee);

            Assert.Equal(2, john.OrganizationWhereEmployee.Length);
            Assert.Contains(acme, john.OrganizationWhereEmployee);
            Assert.Contains(hooli, john.OrganizationWhereEmployee);

            Assert.Equal(2, jenny.OrganizationWhereEmployee.Length);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);
            Assert.Contains(hooli, jenny.OrganizationWhereEmployee);

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