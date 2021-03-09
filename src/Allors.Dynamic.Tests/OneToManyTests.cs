namespace Allors.Dynamic.Tests
{
    using System.Linq;
    using Allors.Dynamic.Meta;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class OneToManyTests
    {
        [Fact]
        public void AddSameAssociation()
        {
            var population = new Default.DynamicPopulation(
                 new DynamicMeta(new Pluralizer()),
                 v => v.AddOneToMany<Organization, Person>("Employee"));

            dynamic acme = population.New<Organization>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(acme, jane.OrganizationWhereEmployee);
            Assert.Equal(acme, john.OrganizationWhereEmployee);
            Assert.Equal(acme, jenny.OrganizationWhereEmployee);
        }

        [Fact]
        public void AddDifferentAssociation()
        {
            var population = new Default.DynamicPopulation(
                 new DynamicMeta(new Pluralizer()),
                 v =>
            {
                v.AddUnit<INamed, string>("Name");
                v.AddOneToMany<Organization, Person>("Employee");
            });

            dynamic acme = population.New<Organization>();

            dynamic jane = population.New<Person>();
            jane.Name = "Jane";
            dynamic john = population.New<Person>();
            john.Name = "John";
            dynamic jenny = population.New<Person>();
            jenny.Name = "Jenny";

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            dynamic hooli = population.New<Organization>();

            hooli.AddEmployee(jane);

            var people = new[] { jane, john, jenny };

            var x = people.Where(v => "Jane".Equals(v.FirstName));

            Assert.Contains(jane, hooli.Employees);

            Assert.DoesNotContain(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(hooli, jane.OrganizationWhereEmployee);

            Assert.NotEqual(acme, jane.OrganizationWhereEmployee);
            Assert.Equal(acme, john.OrganizationWhereEmployee);
            Assert.Equal(acme, jenny.OrganizationWhereEmployee);
        }

        [Fact]
        public void Remove()
        {
            var population = new Default.DynamicPopulation(
                 new DynamicMeta(new Pluralizer()),
                 v => v.AddOneToMany<Organization, Person>("Employee"));

            dynamic acme = population.New<Organization>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            acme.RemoveEmployee(jane);

            Assert.DoesNotContain(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.NotEqual(acme, jane.OrganizationWhereEmployee);
            Assert.Equal(acme, john.OrganizationWhereEmployee);
            Assert.Equal(acme, jenny.OrganizationWhereEmployee);

            acme.RemoveEmployee(john);

            Assert.DoesNotContain(jane, acme.Employees);
            Assert.DoesNotContain(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.NotEqual(acme, jane.OrganizationWhereEmployee);
            Assert.NotEqual(acme, john.OrganizationWhereEmployee);
            Assert.Equal(acme, jenny.OrganizationWhereEmployee);

            acme.RemoveEmployee(jenny);

            Assert.DoesNotContain(jane, acme.Employees);
            Assert.DoesNotContain(john, acme.Employees);
            Assert.DoesNotContain(jenny, acme.Employees);

            Assert.NotEqual(acme, jane.OrganizationWhereEmployee);
            Assert.NotEqual(acme, john.OrganizationWhereEmployee);
            Assert.NotEqual(acme, jenny.OrganizationWhereEmployee);
        }
    }
}
