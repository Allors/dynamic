using Xunit;

namespace Allors.Dynamic.Tests
{
    public class SnapshotTests
    {
        [Fact]
        public void Snapshot()
        {
            var population = new Population();

            dynamic john = population.NewObject();
            dynamic jane = population.NewObject();

            john.FirstName = "John";
            john.LastName = "Doe";

            var snapshot1 = population.Snapshot();

            jane.FirstName = "Jane";
            jane.LastName = "Doe";

            var changedFirstNames = snapshot1.ChangedRoles("FirstName");
            var changedLastNames = snapshot1.ChangedRoles("LastName");

            Assert.Single(changedFirstNames.Keys);
            Assert.Single(changedLastNames.Keys);
            Assert.Contains(john, changedFirstNames.Keys);
            Assert.Contains(john, changedLastNames.Keys);

            var snapshot2 = population.Snapshot();

            changedFirstNames = snapshot2.ChangedRoles("FirstName");
            changedLastNames = snapshot2.ChangedRoles("LastName");

            Assert.Single(changedFirstNames.Keys);
            Assert.Single(changedLastNames.Keys);
            Assert.Contains(jane, changedFirstNames.Keys);
            Assert.Contains(jane, changedLastNames.Keys);
        }
    }
}
