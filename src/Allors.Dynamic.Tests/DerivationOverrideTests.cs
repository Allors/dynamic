namespace Allors.Dynamic.Tests
{
    using System;
    using System.Linq;
    using Xunit;

    public class DerivationOverrideTests
    {
        [Fact]
        public void Derivation()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                  .AddUnitRelationType("FirstName")
                  .AddUnitRelationType("LastName")
                  .AddUnitRelationType("FullName")
                  .AddUnitRelationType("DerivedAt")
                  .AddUnitRelationType("Greeting")
       );

            population.DerivationById["FullName"] = new FullNameDerivation();
            population.DerivationById["Greeting"] = new GreetingDerivation();

            dynamic john = population.Create();
            john.FirstName = "John";
            john.LastName = "Doe";

            population.Derive();

            Assert.Equal("Hello John Doe!", john.Greeting);
        }

        public class FullNameDerivation : IDynamicDerivation
        {
            public void Derive(DynamicChangeSet changeSet)
            {
                System.Collections.Generic.Dictionary<DynamicObject, object> firstNames = changeSet.ChangedRoles("FirstName");
                System.Collections.Generic.Dictionary<DynamicObject, object> lastNames = changeSet.ChangedRoles("LastName");

                if (firstNames?.Any() == true || lastNames?.Any() == true)
                {
                    System.Collections.Generic.IEnumerable<DynamicObject> people = firstNames.Union(lastNames).Select(v => v.Key).Distinct();

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

        public class GreetingDerivation : IDynamicDerivation
        {
            public void Derive(DynamicChangeSet changeSet)
            {
                System.Collections.Generic.Dictionary<DynamicObject, object> fullNames = changeSet.ChangedRoles("FullName");

                if (fullNames?.Any() == true)
                {
                    System.Collections.Generic.IEnumerable<DynamicObject> people = fullNames.Select(v => v.Key).Distinct();

                    foreach (dynamic person in people)
                    {
                        person.Greeting = $"Hello {person.FullName}!";
                    }
                }
            }
        }
    }
}
