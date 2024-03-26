namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Domain;
    using Allors.Dynamic.Meta;
    using Xunit;

    public class UnitTests
    {
        [Fact]
        public void SameRoleTypeName()
        {
            var meta = new DynamicMeta();
            var c1 = meta.AddClass("C1");
            var c2 = meta.AddClass("C2");
            meta.AddUnit<string>(c1, "Same");
            meta.AddUnit<string>(c2, "Same");

            var population = new DynamicPopulation();

            var c1a = population.Create(c1, v =>
            {
                v["Same"] = "c1";
            });

            var c2a = population.Create(c2, v =>
            {
                v["Same"] = "c2";
            });

            Assert.Equal("c1", c1a["Same"]);
            Assert.Equal("c2", c2a["Same"]);
        }

        [Fact]
        public void PropertySet()
        {
            var meta = new DynamicMeta();
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(person, "FirstName");

            var population = new DynamicPopulation();

            var john = population.Create(person);
            var jane = population.Create(person);

            john["FirstName"] = "John";
            jane["FirstName"] = "Jane";

            Assert.Equal("John", john["FirstName"]);
            Assert.Equal("Jane", jane["FirstName"]);
        }
    }
}
