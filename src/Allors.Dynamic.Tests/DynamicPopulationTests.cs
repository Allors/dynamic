namespace Allors.Dynamic.Tests
{
    using Xunit;

    public class DynamicPopulationTests
    {
        [Fact]
        public void New()
        {
            var population = new DynamicPopulation();
            var name = population.Meta.AddUnit<string>("Name");
            var (property, owner) = population.Meta.AddOneToOne("Property", "Owner");

            New @new = population.New;
            var setName = name.Set();
            var setOwner = owner.Set();

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
