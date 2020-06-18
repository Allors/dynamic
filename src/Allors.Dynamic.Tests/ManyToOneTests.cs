namespace Allors.Dynamic.Tests
{
    using Xunit;

    public class ManyToOneTests
    {
        [Fact]
        public void PropertySet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                .AddOneToOne("Property", "Owner"));

            dynamic acme = population.New();
            dynamic gizmo = population.New();
            dynamic jane = population.New();
            dynamic john = population.New();

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
                .AddOneToOne("Property", "Owner"));

            dynamic acme = population.New();
            dynamic gizmo = population.New();
            dynamic jane = population.New();
            dynamic john = population.New();

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
