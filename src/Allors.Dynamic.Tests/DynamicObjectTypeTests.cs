namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Meta;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class DynamicObjectTypeTests
    {
        [Fact]
        public void SameUnitTypeName()
        {
            var population = new Default.DynamicPopulation(new DynamicMeta(new Pluralizer()));
            var c1Same = population.Meta.AddUnit<C1, string>("Same");
            var c2Same = population.Meta.AddUnit<C2, string>("Same");

            New<C1> newC1 = population.New;
            New<C2> newC2 = population.New;

            var c1 = newC1(v =>
            {
                v.Same("c1");
            });

            var c2 = newC2(v =>
            {
                v.Same("c2");
            });

            Assert.Equal("c1", c1.Same());
            Assert.Equal("c2", c2.Same());
        }
    }
}
