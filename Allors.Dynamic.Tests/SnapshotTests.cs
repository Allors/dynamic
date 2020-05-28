using Xunit;

namespace Allors.Dynamic.Tests
{
    public class SnapshotTests
    {
        [Fact]
        public void Snapshot()
        {
            var population = new DynamicPopulation(v => v
                .AddDataAssociation("FirstName")
                .AddDataAssociation("LastName")
             );

            dynamic john = population.Create();
            dynamic jane = population.Create();

            john.FirstName = "John";
            john.LastName = "Doe";

            var snapshot1 = population.Snapshot();

            jane.FirstName = "Jane";
            jane.LastName = "Doe";

            var changedFirstNames = snapshot1.ChangedLinked("FirstName");
            var changedLastNames = snapshot1.ChangedLinked("LastName");

            Assert.Single(changedFirstNames.Keys);
            Assert.Single(changedLastNames.Keys);
            Assert.Contains(john, changedFirstNames.Keys);
            Assert.Contains(john, changedLastNames.Keys);

            var snapshot2 = population.Snapshot();

            changedFirstNames = snapshot2.ChangedLinked("FirstName");
            changedLastNames = snapshot2.ChangedLinked("LastName");

            Assert.Single(changedFirstNames.Keys);
            Assert.Single(changedLastNames.Keys);
            Assert.Contains(jane, changedFirstNames.Keys);
            Assert.Contains(jane, changedLastNames.Keys);
        }
    }
}
