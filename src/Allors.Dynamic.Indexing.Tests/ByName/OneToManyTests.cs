namespace Allors.Dynamic.Indexing.Tests.ByName
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
            meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);
            var jane = population.New(person);
            var john = population.New(person);
            var jenny = population.New(person);

            acme.AddRole("Employee", jane);
            acme.AddRole("Employee", john);
            acme.AddRole("Employee", jenny);

            Assert.Contains(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

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
            meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);

            var jane = population.New(person);
            jane["Name"] = "Jane";
            var john = population.New(person);
            john["Name"] = "John";
            var jenny = population.New(person);
            jenny["Name"] = "Jenny";

            acme.AddRole("Employee", jane);
            acme.AddRole("Employee", john);
            acme.AddRole("Employee", jenny);

            var hooli = population.New(organization);

            hooli.AddRole("Employee", jane);

            var people = new[] { jane, john, jenny };

            var x = people.Where(v => "Jane".Equals(v["FirstName"]));

            Assert.Contains(jane, (IEnumerable<IDynamicObject>)hooli["Employees"]);

            Assert.DoesNotContain(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

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
            meta.AddOneToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);
            var jane = population.New(person);
            var john = population.New(person);
            var jenny = population.New(person);

            acme.AddRole("Employee", jane);
            acme.AddRole("Employee", john);
            acme.AddRole("Employee", jenny);

            acme.RemoveRole("Employee", jane);

            Assert.DoesNotContain(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.Equal(acme, john["OrganizationWhereEmployee"]);
            Assert.Equal(acme, jenny["OrganizationWhereEmployee"]);

            acme.RemoveRole("Employee", john);

            Assert.DoesNotContain(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.DoesNotContain(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.Contains(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, john["OrganizationWhereEmployee"]);
            Assert.Equal(acme, jenny["OrganizationWhereEmployee"]);

            acme.RemoveRole("Employee", jenny);

            Assert.DoesNotContain(jane, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.DoesNotContain(john, (IEnumerable<IDynamicObject>)acme["Employees"]);
            Assert.DoesNotContain(jenny, (IEnumerable<IDynamicObject>)acme["Employees"]);

            Assert.NotEqual(acme, jane["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, john["OrganizationWhereEmployee"]);
            Assert.NotEqual(acme, jenny["OrganizationWhereEmployee"]);
        }
    }
}
