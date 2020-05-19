using Xunit;

namespace Allors.Dynamic.Tests
{
    public class UnitRelationTests
    {
        [Fact]
        public void StringRelation()
        {
            var population = new Population();

            dynamic jubayer = population.NewObject();
            dynamic walter = population.NewObject();

            jubayer.FirstName = "Jubayer";
            walter.FirstName = "Walter";

            Assert.Equal("Jubayer", jubayer.FirstName);
            Assert.Equal("Walter", walter.FirstName);
        }
    }
}
