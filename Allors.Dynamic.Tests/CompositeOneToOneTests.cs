using Xunit;

namespace Allors.Dynamic.Tests
{
    public class CompositeOneToOneTests
    {
        [Fact]
        public void Set()
        {
            var population = new DynamicPopulation(v => v
                .AddCompositeRelation("Property", false, "Owner", false)
             );

            dynamic acme = population.NewObject();
            dynamic gizmo = population.NewObject();

            dynamic jane = population.NewObject();
            dynamic john = population.NewObject();

            acme.Owner = jane;
            gizmo.Owner = john;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(john, gizmo.Owner);

            Assert.Equal(acme, jane.Property);
            Assert.Equal(gizmo, john.Property);

            gizmo.Owner = jane;

            Assert.Null(acme.Owner);
            Assert.Equal(jane, gizmo.Owner);

            Assert.Equal(gizmo, jane.Property);
            Assert.Null(john.Property);
        }
    }
}
