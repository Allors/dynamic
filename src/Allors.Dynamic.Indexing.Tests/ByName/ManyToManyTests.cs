using System;
using System.Collections.Generic;
using System.Linq;
using Allors.Dynamic.Meta;
using Xunit;

namespace Allors.Dynamic.Indexing.Tests.ByName
{
    public class ManyToManyTests
    {
        [Fact]
        public void AddSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization, v => v["Name"] = "Acme");
            var hooli = population.New(organization, v => v["Name"] = "Hooli");

            var jane = population.New(person);
            var john = population.New(person);
            var jenny = population.New(person);

            acme.AddRole("Employee", jane);
            acme.AddRole("Employee", john);
            acme.AddRole("Employee", jenny);

            Assert.Single((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<IDynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<IDynamicObject>)hooli["Employees"]);
        }

        [Fact]
        public void SetSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization, v => v["Name"] = "Acme");
            var hooli = population.New(organization, v => v["Name"] = "Hooli");

            var jane = population.New(person);
            var john = population.New(person);
            var jenny = population.New(person);

            acme["Employees"] = new[] { jane };

            Assert.Single((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<IDynamicObject>)hooli["Employees"]);

            acme["Employees"] = new[] { jane, john };

            Assert.Single((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(2, ((IEnumerable<IDynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<IDynamicObject>)hooli["Employees"]);

            acme["Employees"] = new[] { jane, john, jenny };

            Assert.Single((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<IDynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<IDynamicObject>)hooli["Employees"]);

            acme["Employees"] = Array.Empty<dynamic>();

            Assert.Empty((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Empty((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Empty((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Empty((IEnumerable<IDynamicObject>)hooli["Employees"]);
        }

        [Fact]
        public void RemoveSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization, v => v["Name"] = "Acme");
            var hooli = population.New(organization, v => v["Name"] = ("Hooli"));

            var jane = population.New(person);
            var john = population.New(person);
            var jenny = population.New(person);

            acme["Employees"] = new[] { jane, john, jenny };

            acme.RemoveRole("Employee", jenny);

            Assert.Single((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(2, (((IEnumerable<IDynamicObject>)acme["Employees"]).Count()));
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<IDynamicObject>)hooli["Employees"]);

            acme.RemoveRole("Employee", john);

            Assert.Single((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(1, (((IEnumerable<IDynamicObject>)acme["Employees"]).Count()));
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Empty((IEnumerable<IDynamicObject>)hooli["Employees"]);

            acme.RemoveRole("Employee", jane);

            Assert.Empty((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Empty((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Empty((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Empty((IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Empty((IEnumerable<IDynamicObject>)hooli["Employees"]);
        }

        [Fact]
        public void MultipeleActiveLinks()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization, v => v["Name"] = "Acme");
            var hooli = population.New(organization, v => v["Name"] = "Hooli");

            var jane = population.New(person);
            var john = population.New(person);
            var jenny = population.New(person);

            acme.AddRole("Employee", jane);
            acme.AddRole("Employee", john);
            acme.AddRole("Employee", jenny);

            hooli.AddRole("Employee", jane);

            Assert.Equal(2, (((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]).Count()));
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<IDynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Single((IEnumerable<IDynamicObject>)hooli["Employees"]);
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)hooli["Employees"]);

            hooli.AddRole("Employee", john);

            Assert.Equal(2, ((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Equal(2, ((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Single((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<IDynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Equal(2, ((IEnumerable<IDynamicObject>)hooli["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)hooli["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)hooli["Employees"]);

            hooli.AddRole("Employee", jenny);

            Assert.Equal(2, ((IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<IDynamicObject>)jane["OrganizationWhereEmployee"]);

            Assert.Equal(2, ((IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<IDynamicObject>)john["OrganizationWhereEmployee"]);

            Assert.Equal(2, ((IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]).Count());
            Assert.Contains(acme, (IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);
            Assert.Contains(hooli, (IEnumerable<IDynamicObject>)jenny["OrganizationWhereEmployee"]);

            Assert.Equal(3, ((IEnumerable<IDynamicObject>)acme["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.Equal(3, ((IEnumerable<IDynamicObject>)hooli["Employees"]).Count());
            Assert.Contains(jane, (IEnumerable<IDynamicObject>)hooli["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)hooli["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)hooli["Employees"]);
        }
    }
}