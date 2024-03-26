using System;
using System.Collections.Generic;
using System.Linq;
using Allors.Dynamic.Domain;
using Allors.Dynamic.Meta;
using Xunit;
using DynamicObject = Allors.Dynamic.Domain.DynamicObject;

namespace Allors.Dynamic.Tests.Domain
{
    public class ManyToManyTests
    {
        [Fact]
        public void AddSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            var name = meta.AddUnit<string>(organization, "Name");
            var (employees, organizationWhereEmployee) = meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization, v => v[name] = "Acme");
            var hooli = population.Create(organization, v => v[name] = "Hooli");

            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.Add(employees, jane);
            acme.Add(employees, john);
            acme.Add(employees, jenny);

            Assert.Single(jane[organizationWhereEmployee]);
            Assert.Contains(acme, jane[organizationWhereEmployee]);

            Assert.Single(john[organizationWhereEmployee]);
            Assert.Contains(acme, john[organizationWhereEmployee]);

            Assert.Single(jenny[organizationWhereEmployee]);
            Assert.Contains(acme, jenny[organizationWhereEmployee]);

            Assert.Equal(3, acme[employees].Count());
            Assert.Contains(jane, acme[employees]);
            Assert.Contains(john, acme[employees]);
            Assert.Contains(jenny, acme[employees]);

            Assert.Empty(hooli[employees]);
        }

        [Fact]
        public void SetSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            var name = meta.AddUnit<string>(organization, "Name");
            var (employees, organizationWhereEmployee) = meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization, v => v[name] = "Acme");
            var hooli = population.Create(organization, v => v[name] = "Hooli");

            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme[employees] = new[] { jane };

            Assert.Single((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<DynamicObject>)hooli["Employees"]);

            acme["Employees"] = new[] { jane, john };

            Assert.Single((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(2, ((IEnumerable<DynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<DynamicObject>)hooli["Employees"]);

            acme["Employees"] = new[] { jane, john, jenny };

            Assert.Single((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<DynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<DynamicObject>)hooli["Employees"]);

            acme["Employees"] = Array.Empty<dynamic>();

            Assert.Empty((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Empty((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Empty((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Empty((IEnumerable<DynamicObject>)hooli["Employees"]);
        }

        [Fact]
        public void RemoveSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            var (employees, organizationWhereEmployee) = meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization, v => v["Name"] = "Acme");
            var hooli = population.Create(organization, v => v["Name"] = ("Hooli"));

            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme["Employees"] = new[] { jane, john, jenny };

            acme.Remove(employees, jenny);

            Assert.Single((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(2, (((IEnumerable<DynamicObject>)acme["Employees"]).Count()));
            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<DynamicObject>)hooli["Employees"]);

            acme.Remove(employees, john);

            Assert.Single((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(1, (((IEnumerable<DynamicObject>)acme["Employees"]).Count()));
            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<DynamicObject>)hooli["Employees"]);

            acme.Remove(employees, jane);

            Assert.Empty((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Empty((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Empty((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Empty((IEnumerable<DynamicObject>)hooli["Employees"]);
        }

        [Fact]
        public void MultipeleActiveLinks()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            var (employees, organizationWhereEmployee) = meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization, v => v["Name"] = "Acme");
            var hooli = population.Create(organization, v => v["Name"] = "Hooli");

            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.Add(employees, jane);
            acme.Add(employees, john);
            acme.Add(employees, jenny);

            hooli.Add(employees, jane);

            Assert.Equal(2, (((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]).Count()));
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<DynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)acme["Employees"]);

            Assert.Single((IEnumerable<DynamicObject>)hooli["Employees"]);
            Assert.Contains(jane, (IEnumerable<DynamicObject>)hooli["Employees"]);

            hooli.Add(employees, john);

            Assert.Equal(2, ((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Equal(2, ((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<DynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)acme["Employees"]);

            Assert.Equal(2, ((IEnumerable<DynamicObject>)hooli["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<DynamicObject>)hooli["Employees"]);
            Assert.Contains(john, (IEnumerable<DynamicObject>)hooli["Employees"]);

            hooli.Add(employees, jenny);

            Assert.Equal(2, ((IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<DynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Equal(2, ((IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<DynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Equal(2, ((IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<DynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<DynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<DynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)acme["Employees"]);

            Assert.Equal(3, ((IEnumerable<DynamicObject>)hooli["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<DynamicObject>)hooli["Employees"]);
            Assert.Contains(john, (IEnumerable<DynamicObject>)hooli["Employees"]);
            Assert.Contains(jenny, (IEnumerable<DynamicObject>)hooli["Employees"]);
        }
    }
}
