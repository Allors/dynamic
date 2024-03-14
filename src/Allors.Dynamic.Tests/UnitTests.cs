using Allors.Dynamic.Meta;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class UnitTests
    {
        [Fact]
        public void PropertySet()
        {
            var meta = new DynamicMeta();
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(person, "FirstName");

            var population = new DynamicPopulation(meta);

            var john = population.New(person);
            var jane = population.New(person);

            john.FirstName = "John";
            jane.FirstName = "Jane";

            Assert.Equal("John", john.FirstName);
            Assert.Equal("Jane", jane.FirstName);
        }
    }
}
