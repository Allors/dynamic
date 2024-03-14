using System;

namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Meta;
    using Allors.Dynamic.Tests.Domain;
    using Xunit;

    public class SnapshotTests
    {
        [Fact]
        public void Unit()
        {
            var population = new Default.DynamicPopulation(
                new DynamicMeta(),
                v =>
            {
                v.AddUnit<Person, string>("FirstName");
                v.AddUnit<Person, string>("LastName");
            });

            dynamic john = population.New<Person>();
            dynamic jane = population.New<Person>();

            john.FirstName = "John";
            john.LastName = "Doe";

            var snapshot1 = population.Snapshot();

            jane.FirstName = "Jane";
            jane.LastName = "Doe";

            var changedFirstNames = snapshot1.ChangedRoles<Person>("FirstName");
            var changedLastNames = snapshot1.ChangedRoles<Person>("LastName");

            Assert.Single(changedFirstNames.Keys);
            Assert.Single(changedLastNames.Keys);
            Assert.Contains(john, changedFirstNames.Keys);
            Assert.Contains(john, changedLastNames.Keys);

            var snapshot2 = population.Snapshot();

            changedFirstNames = snapshot2.ChangedRoles<Person>("FirstName");
            changedLastNames = snapshot2.ChangedRoles<Person>("LastName");

            Assert.Single(changedFirstNames.Keys);
            Assert.Single(changedLastNames.Keys);
            Assert.Contains(jane, changedFirstNames.Keys);
            Assert.Contains(jane, changedLastNames.Keys);
        }


        [Fact]
        public void Composites()
        {
            var population = new Default.DynamicPopulation(
                new DynamicMeta(),
                v =>
                {
                    v.AddUnit<Person, string>("FirstName");
                    v.AddUnit<Person, string>("LastName");
                    v.AddUnit<Organization, string>("Name");
                    v.AddManyToMany<Organization, Person>("Employee");
                });

            dynamic john = population.New<Person>();
            dynamic jane = population.New<Person>();

            john.FirstName = "John";
            john.LastName = "Doe";

            jane.FirstName = "Jane";
            jane.LastName = "Doe";

            dynamic acme = population.New<Organization>();

            acme.Name = "Acme";

            acme.Employees = new[] { john, jane };

            var snapshot = population.Snapshot();
            var changedEmployees = snapshot.ChangedRoles<Organization>("Employee");
            Assert.Single(changedEmployees);

            acme.Employees = new[] { jane, john };

            snapshot = population.Snapshot();
            changedEmployees = snapshot.ChangedRoles<Organization>("Employee");
            Assert.Empty(changedEmployees);

            acme.Employees = Array.Empty<DynamicObject>();

            var x = acme.Employees;

            acme.Employees = new[] { jane, john };

            snapshot = population.Snapshot();
            changedEmployees = snapshot.ChangedRoles<Organization>("Employee");
            Assert.Empty(changedEmployees);
        }
    }
}
