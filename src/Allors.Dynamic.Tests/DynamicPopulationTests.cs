namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class DynamicPopulationTests
    {
        [Fact]
        public void New()
        {
            var population = new DynamicPopulation(new Pluralizer());
            var name = population.Meta.AddUnit<Named, string>("Name");
            var (property, owner) = population.Meta.AddOneToOne<Organisation, Person>("OrganisationWhereOwner", "Owner");

            New<Organisation> newOrganisation = population.New;
            New<Person> newPerson = population.New;

            var acme = newOrganisation(v =>
            {
                v.Name("Acme");
                v.Owner(newPerson(v => v.Name("Jane")));
            });

            var jane = acme.Owner();

            Assert.Equal("Acme", acme.Name());
            Assert.Equal("Jane", jane.Name());

            Assert.Equal(acme, jane.OrganisationWhereOwner());
        }
    }
}
