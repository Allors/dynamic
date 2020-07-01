namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class ManyToOneTests
    {
        [Fact]
        public void PropertySet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                .AddOneToOne<Organisation, Person>("Property", "Owner"));

            dynamic acme = population.New<Organisation>();
            dynamic gizmo = population.New<Organisation>();

            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme.Owner = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.Property);

            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["Property"]);

            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["Property"]);
        }

        [Fact]
        public void IndexSet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                .AddOneToOne<Organisation, Person>("Property", "Owner"));

            dynamic acme = population.New<Organisation>();
            dynamic gizmo = population.New<Organisation>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();

            acme["Owner"] = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.Property);

            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["Property"]);

            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["Property"]);
        }
    }
}
