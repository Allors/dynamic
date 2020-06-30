namespace Allors.Dynamic.Tests
{
    using Xunit;

    public class DataTests
    {
        [Fact]
        public void PropertySet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                 .AddUnit<string>("FirstName")
              );

            dynamic jubayer = population.New();
            dynamic walter = population.New();

            jubayer.FirstName = "Jubayer";
            walter.FirstName = "Walter";

            Assert.Equal("Jubayer", jubayer.FirstName);
            Assert.Equal("Walter", walter.FirstName);
        }


        [Fact]
        public void IndexSet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                 .AddUnit<string>("FirstName")
              );

            dynamic jubayer = population.New();
            dynamic walter = population.New();

            jubayer.FirstName = "Jubayer";
            walter.FirstName = "Walter";

            Assert.Equal("Jubayer", jubayer.FirstName);
            Assert.Equal("Walter", walter.FirstName);
        }
    }
}
