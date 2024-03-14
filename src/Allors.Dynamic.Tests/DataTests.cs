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
            var population = new DynamicPopulation(
                new DynamicMeta(),
                v => v.AddUnit<Person, string>("FirstName"));

            dynamic john = population.New<Person>();
            dynamic jane = population.New<Person>();

            john.FirstName = "John";
            jane.FirstName = "Jane";

            Assert.Equal("John", john.FirstName);
            Assert.Equal("Jane", jane.FirstName);
        }

        [Fact]
        public void IndexSet()
        {
            var meta = new DynamicMeta();
            meta.AddUnit<Person, string>("FirstName");
            var population = new DynamicPopulation(meta);

            dynamic john = population.New<Person>();
            dynamic jane = population.New<Person>();

            john.FirstName = "John";
            jane.FirstName = "Jane";

            Assert.Equal("John", john.FirstName);
            Assert.Equal("Jane", jane.FirstName);
        }
    }
}
