namespace Allors.Dynamic.Tests
{
    using System;
    using Allors.Dynamic.Meta;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class OneToOneTests
    {
        [Fact]
        public void StaticPropertySet()
        {
            var population = new Default.DynamicPopulation(
                 new DynamicMeta(),
                 v =>
            {
                v.AddOneToOne<Organization, INamed>("Named");
                v.AddOneToOne<Organization, Person>("Owner");
            });

            var acme = population.New<Organization>();
            var gizmo = population.New<Organization>();

            var jane = population.New<Person>();
            var john = population.New<Person>();

            acme.Owner(jane);

            Assert.Equal(jane, acme.Owner());
            Assert.Equal(acme, jane.OrganizationWhereOwner());

            Assert.Null(gizmo.Owner());
            Assert.Null(john.OrganizationWhereOwner());

            acme.Named(jane);

            Assert.Equal(jane, acme.Named());
            Assert.Equal(acme, jane.OrganizationWhereNamed());

            Assert.Null(gizmo.Named());
            Assert.Null(john.OrganizationWhereNamed());
        }

        [Fact]
        public void DynamicPropertySet()
        {
            var meta = new DynamicMeta();
            var population = new Default.DynamicPopulation(meta);
            var (property, owner) = meta.AddOneToOne<Organization, Person>("Owner");
            var (organisation, named) = meta.AddOneToOne<Organization, Person>("Named");

            dynamic acme = population.New<Organization>();
            dynamic gizmo = population.New<Organization>();

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme.Owner = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.OrganizationWhereOwner);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.OrganizationWhereOwner);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganizationWhereOwner"]);
            Assert.Null(gizmo[owner]);
            Assert.Null(john[property]);

            // Wrong Type
            Assert.Throws<ArgumentException>(() =>
            {
                acme.Owner = gizmo;
            });
        }

        [Fact]
        public void IndexByNameSet()
        {
            var meta = new DynamicMeta();
            var population = new Default.DynamicPopulation(meta);
            var (property, owner) = meta.AddOneToOne<Organization, Person>("Owner");
            var (organisation, named) = meta.AddOneToOne<Organization, Person>("Named");

            dynamic acme = population.New<Organization>();
            dynamic gizmo = population.New<Organization>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme["Owner"] = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.OrganizationWhereOwner);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.OrganizationWhereOwner);
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
            var population = new Default.DynamicPopulation(meta);
            var (property, owner) = meta.AddOneToOne<Organization, Person>("Owner");
            var (organisation, named) = meta.AddOneToOne<Organization, Person>("Named");

            dynamic acme = population.New<Organization>();
            dynamic gizmo = population.New<Organization>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme[owner] = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.OrganizationWhereOwner);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganizationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.OrganizationWhereOwner);
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
