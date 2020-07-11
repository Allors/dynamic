namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Meta;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class DataTests
    {
        [Fact]
        public void PropertySet()
        {
            var population = new Default.DynamicPopulation(
                new DynamicMeta(new Pluralizer()),
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
            var meta = new DynamicMeta(new Pluralizer());
            meta.AddUnit<Person, string>("FirstName");
            var population = new Default.DynamicPopulation(meta);

            dynamic jubayer = population.New<Person>();
            dynamic walter = population.New<Person>();

            jubayer.FirstName = "Jubayer";
            walter.FirstName = "Walter";

            Assert.Equal("Jubayer", jubayer.FirstName);
            Assert.Equal("Walter", walter.FirstName);
        }
    }
}
