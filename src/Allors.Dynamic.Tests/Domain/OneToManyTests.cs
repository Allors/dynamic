namespace Allors.Dynamic.Tests.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using Allors.Dynamic.Domain;
    using Allors.Dynamic.Meta;
    using Xunit;
    using DynamicObject = Allors.Dynamic.Domain.DynamicObject;

    public class OneToManyTests
    {
        [Fact]
        public void AddSameAssociation()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            (DynamicOneToManyRoleType employees, _) = meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation();

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
            (DynamicOneToManyRoleType employees, _) = meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation();

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
            (DynamicOneToManyRoleType employees, _) = meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation();

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

        [Fact]
        public void RemoveAll()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            (DynamicOneToManyRoleType employees, _) = meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation();

            var acme = population.Create(organization);
            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.Add(employees, jane);
            acme.Add(employees, john);
            acme.Add(employees, jenny);

            acme["Employees"] = null;

            Assert.DoesNotContain(jane, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(john, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(jenny, (IEnumerable<DynamicObject>)acme["Employees"]!);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, john["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, jenny["OrganizationWhereEmployee"]);

            acme.Add(employees, jane);
            acme.Add(employees, john);
            acme.Add(employees, jenny);

            acme["Employees"] = Array.Empty<DynamicObject>();

            Assert.DoesNotContain(jane, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(john, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(jenny, (IEnumerable<DynamicObject>)acme["Employees"]!);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, john["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, jenny["OrganizationWhereEmployee"]);

            acme.Add(employees, jane);
            acme.Add(employees, john);
            acme.Add(employees, jenny);

            acme["Employees"] = ImmutableHashSet<DynamicObject>.Empty;

            Assert.DoesNotContain(jane, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(john, (IEnumerable<DynamicObject>)acme["Employees"]!);
            Assert.DoesNotContain(jenny, (IEnumerable<DynamicObject>)acme["Employees"]!);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, john["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, jenny["OrganizationWhereEmployee"]);
        }
    }
}
