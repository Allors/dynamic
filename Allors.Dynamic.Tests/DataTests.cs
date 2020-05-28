namespace Allors.Dynamic.Tests
{
    using Xunit;

    public class DataTests
    {
        [Fact]
        public void PropertySet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                 .AddUnitRelationType("FirstName")
              );

            dynamic jubayer = population.Create();
            dynamic walter = population.Create();

            jubayer.FirstName = "Jubayer";
            walter.FirstName = "Walter";

            Assert.Equal("Jubayer", jubayer.FirstName);
            Assert.Equal("Walter", walter.FirstName);
        }


        [Fact]
        public void IndexSet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                 .AddUnitRelationType("FirstName")
              );

            dynamic jubayer = population.Create();
            dynamic walter = population.Create();

            jubayer.FirstName = "Jubayer";
            walter.FirstName = "Walter";

            Assert.Equal("Jubayer", jubayer.FirstName);
            Assert.Equal("Walter", walter.FirstName);
        }
    }
}
