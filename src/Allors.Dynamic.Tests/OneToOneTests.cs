namespace Allors.Dynamic.Tests
{
    using System;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class OneToOneTests
    {
        [Fact]
        public void StaticPropertySet()
        {
            DynamicPopulation population = new DynamicPopulation(v =>
            {
                v.AddOneToOne<Organisation, Person>("Property", "Owner");
                v.AddOneToOne<Organisation, Named>("By", "Named");
            });

            var acme = population.New<Organisation>();
            var gizmo = population.New<Organisation>();

            var jane = population.New<Person>();
            var john = population.New<Person>();

            acme.Owner(jane);

            Assert.Equal(jane, acme.Owner());
            Assert.Equal(acme, jane.Property());

            Assert.Null(gizmo.Owner());
            Assert.Null(john.Property());

            acme.Named(jane);

            Assert.Equal(jane, acme.Named());
            Assert.Equal(acme, jane.By());

            Assert.Null(gizmo.Named());
            Assert.Null(john.By());
        }

        [Fact]
        public void DynamicPropertySet()
        {
            DynamicPopulation population = new DynamicPopulation();
            var meta = population.Meta;
            var (property, owner) = meta.AddOneToOne<Organisation, Person>("Property", "Owner");
            var (organisation, named) = meta.AddOneToOne<Organisation, Person>("By", "Named");

            dynamic acme = population.New<Organisation>();
            dynamic gizmo = population.New<Organisation>();

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme.Owner = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.Property);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["Property"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.Property);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["Property"]);
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
            DynamicPopulation population = new DynamicPopulation();
            var meta = population.Meta;
            var (property, owner) = meta.AddOneToOne<Organisation, Person>("Property", "Owner");
            var (organisation, named) = meta.AddOneToOne<Organisation, Person>("By", "Named");

            dynamic acme = population.New<Organisation>();
            dynamic gizmo = population.New<Organisation>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme["Owner"] = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.Property);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["Property"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.Property);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["Property"]);
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
            DynamicPopulation population = new DynamicPopulation();
            var meta = population.Meta;
            var (property, owner) = meta.AddOneToOne<Organisation, Person>("Property", "Owner");
            var (organisation, named) = meta.AddOneToOne<Organisation, Person>("By", "Named");

            dynamic acme = population.New<Organisation>();
            dynamic gizmo = population.New<Organisation>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme[owner] = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.Property);
            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["Property"]);
            Assert.Equal(jane, acme[owner]);
            Assert.Equal(acme, jane[property]);

            Assert.Null(gizmo.Owner);
            Assert.Null(john.Property);
            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["Property"]);
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
