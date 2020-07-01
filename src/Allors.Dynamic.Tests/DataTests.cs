namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class DataTests
    {
        [Fact]
        public void PropertySet()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                 .AddUnit<string>("FirstName")
              );

            dynamic jubayer = population.New<Person>();
            dynamic walter = population.New<Person>();

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

            dynamic jubayer = population.New<Person>();
            dynamic walter = population.New<Person>();

            jubayer.FirstName = "Jubayer";
            walter.FirstName = "Walter";

            Assert.Equal("Jubayer", jubayer.FirstName);
            Assert.Equal("Walter", walter.FirstName);
        }
    }
}
