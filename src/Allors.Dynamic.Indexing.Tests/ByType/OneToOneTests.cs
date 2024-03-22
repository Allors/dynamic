namespace Allors.Dynamic.Indexing.Tests.ByType
{
    using System;
    using Allors.Dynamic.Meta;
    using Xunit;

    public class OneToOneTests
    {
        [Fact]
        public void StaticPropertySet()
        {
            var meta = new DynamicMeta();
            var named = meta.AddInterface("Named");
            var organization = meta.AddClass("Organization", named);
            var person = meta.AddClass("Person", named);
            meta.AddOneToOne(organization, person, "Owner");
            meta.AddOneToOne(organization, named, "Named");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);
            var gizmo = population.New(organization);

            var jane = population.New(person);
            var john = population.New(person);

            acme["Owner"] = jane;

            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);

            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganizationWhereOwner"]);

            acme["Named"] = jane;

            Assert.Equal(jane, acme["Named"]);
            Assert.Equal(acme, jane["OrganizationWhereNamed"]);

            Assert.Null(gizmo["Named"]);
            Assert.Null(john["OrganizationWhereNamed"]);
        }

        [Fact]
        public void DynamicPropertySet()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            var (owner, property) = meta.AddOneToOne(organization, person, "Owner");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);
            var gizmo = population.New(organization);

            var jane = population.New(person);
            var john = population.New(person);

            acme["Owner"] = jane;

            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganizationWhereOwner"]);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganizationWhereOwner"]);
            Assert.Null(gizmo[owner]);
            Assert.Null(john[property]);

            // Wrong Type
            Assert.Throws<ArgumentException>(() =>
            {
                acme["Owner"] = gizmo;
            });
        }

        [Fact]
        public void IndexByNameSet()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            var (owner, property) = meta.AddOneToOne(organization, person, "Owner");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);
            var gizmo = population.New(organization);

            var jane = population.New(person);
            var john = population.New(person);

            acme["Owner"] = jane;

            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganizationWhereOwner"]);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganizationWhereOwner"]);
            Assert.Null(gizmo[owner]);
            Assert.Null(john[property]);

            // Wrong Type
            Assert.Throws<ArgumentException>(() =>
            {
                acme["Owner"] = gizmo;
            });
        }

        [Fact]
        public void IndexByRoleSet()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            var (owner, property) = meta.AddOneToOne(organization, person, "Owner");

            var population = new DynamicPopulation(meta);

            var acme = population.New(organization);
            var gizmo = population.New(organization);

            var jane = population.New(person);
            var john = population.New(person);

            acme[owner] = jane;

            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganizationWhereOwner"]);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganizationWhereOwner"]);
            Assert.Null(gizmo[owner]);
            Assert.Null(john[property]);

            // Wrong Type
            Assert.Throws<ArgumentException>(() =>
            {
                acme[owner] = gizmo;
            });
        }
    }
}
