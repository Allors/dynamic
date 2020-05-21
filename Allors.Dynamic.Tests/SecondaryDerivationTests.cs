using System;
using System.Linq;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class SecondaryDerivationTests
    {
        [Fact]
        public void Derivation()
        {
            var population = new Population();

            population.DerivationById["FullName"] = new FullNameDerivation();
            population.DerivationById["Greeting"] = new GreetingDerivation();

            dynamic john = population.NewObject();
            john.FirstName = "John";
            john.LastName = "Doe";

            population.Derive();

            Assert.Equal("Hello John Doe!", john.Greeting);
        }

        public class FullNameDerivation : IDerivation
        {
            public void Derive(ChangeSet changeSet)
            {
                var firstNames = changeSet.ChangedRoles("FirstName");
                var lastNames = changeSet.ChangedRoles("LastName");

                if (firstNames?.Any() == true || lastNames?.Any() == true)
                {
                    var people = firstNames.Union(lastNames).Select(v => v.Key).Distinct();

                    foreach (dynamic person in people)
                    {
                        // Dummy updates ...
                        person.FirstName = person.FirstName;
                        person.LastName = person.LastName;

                        person.DerivedAt = DateTime.Now;

                        person.FullName = $"{person.FirstName} {person.LastName}";
                    }
                }
            }
        }

        public class GreetingDerivation : IDerivation
        {
            public void Derive(ChangeSet changeSet)
            {
                var fullNames = changeSet.ChangedRoles("FullName");

                if (fullNames?.Any() == true)
                {
                    var people = fullNames.Select(v => v.Key).Distinct();

                    foreach (dynamic person in people)
                    {
                        person.Greeting = $"Hello {person.FullName}!";
                    }
                }
            }
        }
    }
}
