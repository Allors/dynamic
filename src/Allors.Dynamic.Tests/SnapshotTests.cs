using System;
using Allors.Dynamic.Meta;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class SnapshotTests
    {
        [Fact]
        public void Unit()
        {
            var meta = new DynamicMeta();
            var person = meta.AddClass("Person");
            var firstName = meta.AddUnit<string>(person, "FirstName");
            var lastName = meta.AddUnit<string>(person, "LastName");

            var population = new DynamicPopulation(meta);

            var john = population.New(person);
            var jane = population.New(person);

            john.FirstName = "John";
            john.LastName = "Doe";

            var snapshot1 = population.Snapshot();

            jane.FirstName = "Jane";
            jane.LastName = "Doe";

            var changedFirstNames = snapshot1.ChangedRoles(firstName);
            var changedLastNames = snapshot1.ChangedRoles(lastName);

            Assert.Single(changedFirstNames.Keys);
            Assert.Single(changedLastNames.Keys);
            Assert.Contains(john, changedFirstNames.Keys);
            Assert.Contains(john, changedLastNames.Keys);

            var snapshot2 = population.Snapshot();

            changedFirstNames = snapshot2.ChangedRoles(firstName);
            changedLastNames = snapshot2.ChangedRoles(lastName);

            Assert.Single(changedFirstNames.Keys);
            Assert.Single(changedLastNames.Keys);
            Assert.Contains(jane, changedFirstNames.Keys);
            Assert.Contains(jane, changedLastNames.Keys);
        }


        [Fact]
        public void Composites()
        {
            var meta = new DynamicMeta();
            var person = meta.AddClass("Person");
            var organization = meta.AddClass("Organization");
            var firstName = meta.AddUnit<string>(person, "FirstName");
            var lastName = meta.AddUnit<string>(person, "LastName");
            var name = meta.AddUnit<string>(organization, "Name");
            var employee = meta.AddManyToMany(organization, person, "Employee");


            var population = new DynamicPopulation(meta);

            var john = population.New(person);
            var jane = population.New(person);

            john.FirstName = "John";
            john.LastName = "Doe";

            jane.FirstName = "Jane";
            jane.LastName = "Doe";

            var acme = population.New(organization);

            acme.Name = "Acme";

            acme.Employees = new[] { john, jane };

            var snapshot = population.Snapshot();
            var changedEmployees = snapshot.ChangedRoles(employee);
            Assert.Single(changedEmployees);

            acme.Employees = new[] { jane, john };

            snapshot = population.Snapshot();
            changedEmployees = snapshot.ChangedRoles(employee);
            Assert.Empty(changedEmployees);

            acme.Employees = Array.Empty<DynamicObject>();

            var x = acme.Employees;

            acme.Employees = new[] { jane, john };

            snapshot = population.Snapshot();
            changedEmployees = snapshot.ChangedRoles(employee);
            Assert.Empty(changedEmployees);
        }
    }
}
