using Xunit;

namespace Allors.Dynamic.Tests
{
    public class CompositeManyToOneTests
    {
        [Fact]
        public void PropertySet()
        {
            var population = new DynamicPopulation(v => v
                .AddOneToOneRelation("Property", "Owner")
             );

            dynamic acme = population.NewObject();
            dynamic gizmo = population.NewObject();
            dynamic jane = population.NewObject();
            dynamic john = population.NewObject();

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
            var population = new DynamicPopulation(v => v
                .AddOneToOneRelation("Property", "Owner")
             );

            dynamic acme = population.NewObject();
            dynamic gizmo = population.NewObject();
            dynamic jane = population.NewObject();
            dynamic john = population.NewObject();

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
