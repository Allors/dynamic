namespace Allors.Dynamic.Tests
{
    using Xunit;

    public class ExtensionsTests
    {
        [Fact]
        public void Set()
        {
            var population = new DynamicPopulation();
            var name = population.Meta.AddUnit("Name");
            var (property, owner) = population.Meta.AddOneToOne("Property", "Owner");

            New @new = population.New;
            var setName = population.Set<string>(name);
            var setOwner = population.Set<dynamic>(owner);

            dynamic acme = @new(
                setName("Acme"),
                setOwner(@new(setName("Jane"))));

            dynamic jane = acme[owner];

            Assert.Equal("Acme", acme[name]);
            Assert.Equal("Jane", jane[name]);

            Assert.Equal(acme, jane[property]);
        }
    }
}
