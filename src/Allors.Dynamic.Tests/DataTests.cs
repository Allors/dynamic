namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class DataTests
    {
        [Fact]
        public void PropertySet()
        {
            var population = new DynamicPopulation(
                new Pluralizer(),
                v => v.AddUnit<Person, string>("FirstName"));

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
            var population = new DynamicPopulation(
              new Pluralizer(),
              v => v.AddUnit<Person, string>("FirstName"));

            dynamic jubayer = population.New<Person>();
            dynamic walter = population.New<Person>();

            jubayer.FirstName = "Jubayer";
            walter.FirstName = "Walter";

            Assert.Equal("Jubayer", jubayer.FirstName);
            Assert.Equal("Walter", walter.FirstName);
        }
    }
}
