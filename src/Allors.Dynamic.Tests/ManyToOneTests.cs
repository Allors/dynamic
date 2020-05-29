namespace Allors.Dynamic.Tests
{
    using Xunit;

    public class ManyToOneTests
    {
        [Fact]
        public void PropertySet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                .AddOneToOneRelationType("Property", "Owner")
             );

            dynamic acme = population.Create();
            dynamic gizmo = population.Create();
            dynamic jane = population.Create();
            dynamic john = population.Create();

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
                .AddOneToOneRelationType("Property", "Owner")
             );

            dynamic acme = population.Create();
            dynamic gizmo = population.Create();
            dynamic jane = population.Create();
            dynamic john = population.Create();

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
