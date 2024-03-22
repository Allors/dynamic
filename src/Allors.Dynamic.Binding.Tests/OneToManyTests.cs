using System.Linq;
using Allors.Dynamic.Meta;
using Xunit;

namespace Allors.Dynamic.Binding.Tests
{
    public class OneToManyTests
    {
        [Fact]
        public void AddSameAssociation()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddOneToMany(organization, person, "Employee");
            
            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);
            var jane = population.New(person);
            var john = population.New(person);
            var jenny = population.New(person);

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
            var meta = new DynamicMeta();
            var named = meta.AddInterface("Named");
            var organization = meta.AddClass("Organization", named);
            var person = meta.AddClass("Person", named);
            meta.AddUnit<string>(named, "Name");
            meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);

            var jane = population.New(person);
            jane.Name = "Jane";
            var john = population.New(person);
            john.Name = "John";
            var jenny = population.New(person);
            jenny.Name = "Jenny";

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            var hooli = population.New(organization);

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
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);
            var jane = population.New(person);
            var john = population.New(person);
            var jenny = population.New(person);

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
