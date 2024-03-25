using System;
using System.Linq;
using Allors.Dynamic.Meta;
using Xunit;

namespace Allors.Dynamic.Indexing.Tests
{
    public class DerivationOverrideTests
    {
        [Fact]
        public void Derivation()
        {
            var meta = new DynamicMeta();
            var person = meta.AddClass("Person");
            var firstName = meta.AddUnit<string>(person, "FirstName");
            var lastName = meta.AddUnit<string>(person, "LastName");
            var fullName = meta.AddUnit<string>(person, "FullName");
            meta.AddUnit<DateTime>(person, "DerivedAt");
            meta.AddUnit<string>(person, "Greeting");

            var population = new DynamicPopulation(meta)
            {
                DerivationById =
                {
                    ["FullName"] = new FullNameDerivation(firstName, lastName),
                    ["Greeting"] = new GreetingDerivation(fullName)
                }
            };

            var john = population.Create(person);
            john["FirstName"] = "John";
            john["LastName"] = "Doe";

            population.Derive();

            Assert.Equal("Hello John Doe!", john["Greeting"]);
        }

        private class FullNameDerivation(IDynamicRoleType firstName, IDynamicRoleType lastName) : IDynamicDerivation
        {
            public void Derive(DynamicChangeSet changeSet)
            {
                var firstNames = changeSet.ChangedRoles(firstName);
                var lastNames = changeSet.ChangedRoles(lastName);

                if (firstNames.Any() || lastNames.Any())
                {
                    var people = firstNames.Union(lastNames).Select(v => v.Key).Distinct();

                    foreach (DynamicObject person in people)
                    {
                        // Dummy updates ...
                        person["FirstName"] = person["FirstName"];
                        person["LastName"] = person["LastName"];

                        person["DerivedAt"] = DateTime.Now;

                        person["FullName"] = $"{person["FirstName"]} {person["LastName"]}";
                    }
                }
            }
        }

        private class GreetingDerivation(IDynamicRoleType fullName) : IDynamicDerivation
        {
            public void Derive(DynamicChangeSet changeSet)
            {
                var fullNames = changeSet.ChangedRoles(fullName);

                if (fullNames.Any())
                {
                    var people = fullNames.Select(v => v.Key).Distinct();

                    foreach (DynamicObject person in people)
                    {
                        person["Greeting"] = $"Hello {person["FullName"]}!";
                    }
                }
            }
        }
    }
}
