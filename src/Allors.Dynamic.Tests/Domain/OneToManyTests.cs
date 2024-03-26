using Allors.Dynamic.Domain;
using DynamicObject = Allors.Dynamic.Domain.DynamicObject;

namespace Allors.Dynamic.Tests.Domain
{
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Dynamic.Meta;
    using Xunit;

    public class OneToManyTests
    {
        [Fact]
        public void AddSameAssociation()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            var (employees, organizationWhereEmployee) = meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization);
            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.Add(employees, jane);
            acme.Add(employees, john);
            acme.Add(employees, jenny);

            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)acme["Employees"]!);

            Assert.Equal(acme, jane["OrganizationWhereEmployee"]);
            Assert.Equal(acme, john["OrganizationWhereEmployee"]);
            Assert.Equal(acme, jenny["OrganizationWhereEmployee"]);
        }

        [Fact]
        public void AddDifferentAssociation()
        {
            var meta = new DynamicMeta();
            var named = meta.AddInterface("Named");
            var organization = meta.AddClass("Organization", named);
            var person = meta.AddClass("Person", named);
            meta.AddUnit<string>(named, "Name");
            var (employees, organizationWhereEmployee) = meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization);

            var jane = population.Create(person);
            jane["Name"] = "Jane";
            var john = population.Create(person);
            john["Name"] = "John";
            var jenny = population.Create(person);
            jenny["Name"] = "Jenny";

            acme.Add(employees, jane);
            acme.Add(employees, john);
            acme.Add(employees, jenny);

            var hooli = population.Create(organization);

            hooli.Add(employees, jane);

            var people = new[] { jane, john, jenny };

            var x = people.Where(v => "Jane".Equals(v["FirstName"]));

            Assert.Contains(jane, (IEnumerable<DynamicObject>)hooli["Employees"]!);

            Assert.DoesNotContain(jane, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)acme["Employees"]!);

            Assert.Equal(hooli, jane["OrganizationWhereEmployee"]);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.Equal(acme, john["OrganizationWhereEmployee"]);
            Assert.Equal(acme, jenny["OrganizationWhereEmployee"]);
        }

        [Fact]
        public void Remove()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            var (employees, organizationWhereEmployee) = meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization);
            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.Add(employees, jane);
            acme.Add(employees, john);
            acme.Add(employees, jenny);

            acme.Remove(employees, jane);

            Assert.DoesNotContain(jane, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)acme["Employees"]!);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.Equal(acme, john["OrganizationWhereEmployee"]);
            Assert.Equal(acme, jenny["OrganizationWhereEmployee"]);

            acme.Remove(employees, john);

            Assert.DoesNotContain(jane, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(john, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)acme["Employees"]!);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, john["OrganizationWhereEmployee"]);
            Assert.Equal(acme, jenny["OrganizationWhereEmployee"]);

            acme.Remove(employees, jenny);

            Assert.DoesNotContain(jane, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(john, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(jenny, (IEnumerable<DynamicObject>)acme["Employees"]!);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, john["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, jenny["OrganizationWhereEmployee"]);
        }
    }
}
