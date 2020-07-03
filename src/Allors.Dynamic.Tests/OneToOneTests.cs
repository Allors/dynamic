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
                 new DynamicMeta(new Pluralizer()),
                 v =>
            {
                v.AddOneToOne<Organisation, Person>("OrganisationWhereOwner", "Owner");
                v.AddOneToOne<Organisation, Named>("By", "Named");
            });

            var acme = population.New<Organisation>();
            var gizmo = population.New<Organisation>();

            var jane = population.New<Person>();
            var john = population.New<Person>();

            acme.Owner(jane);

            Assert.Equal(jane, acme.Owner());
            Assert.Equal(acme, jane.OrganisationWhereOwner());

            Assert.Null(gizmo.Owner());
            Assert.Null(john.OrganisationWhereOwner());

            acme.Named(jane);

            Assert.Equal(jane, acme.Named());
            Assert.Equal(acme, jane.OrganisationWhereNamed());

            Assert.Null(gizmo.Named());
            Assert.Null(john.OrganisationWhereNamed());
        }

        [Fact]
        public void DynamicPropertySet()
        {
            var meta = new DynamicMeta(new Pluralizer());
            var population = new Default.DynamicPopulation(meta);
            var (property, owner) = meta.AddOneToOne<Organisation, Person>("OrganisationWhereOwner", "Owner");
            var (organisation, named) = meta.AddOneToOne<Organisation, Person>("By", "Named");

            dynamic acme = population.New<Organisation>();
            dynamic gizmo = population.New<Organisation>();

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme.Owner = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.OrganisationWhereOwner);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganisationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.OrganisationWhereOwner);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganisationWhereOwner"]);
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
            var meta = new DynamicMeta(new Pluralizer());
            var population = new Default.DynamicPopulation(meta);
            var (property, owner) = meta.AddOneToOne<Organisation, Person>("OrganisationWhereOwner", "Owner");
            var (organisation, named) = meta.AddOneToOne<Organisation, Person>("By", "Named");

            dynamic acme = population.New<Organisation>();
            dynamic gizmo = population.New<Organisation>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme["Owner"] = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.OrganisationWhereOwner);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganisationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.OrganisationWhereOwner);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganisationWhereOwner"]);
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
            var meta = new DynamicMeta(new Pluralizer());
            var population = new Default.DynamicPopulation(meta);
            var (property, owner) = meta.AddOneToOne<Organisation, Person>("OrganisationWhereOwner", "Owner");
            var (organisation, named) = meta.AddOneToOne<Organisation, Person>("By", "Named");

            dynamic acme = population.New<Organisation>();
            dynamic gizmo = population.New<Organisation>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme[owner] = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.OrganisationWhereOwner);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["OrganisationWhereOwner"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.OrganisationWhereOwner);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["OrganisationWhereOwner"]);
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
