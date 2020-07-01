namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Tests.Domain;
    using System;
    using System.Linq;
    using Xunit;

    public class DerivationTests
    {
        [Fact]
        public void Derivation()
        {
            DynamicPopulation population = new DynamicPopulation(v =>
            {
                v.AddUnit<string>("FirstName");
                v.AddUnit<string>("LastName");
                v.AddUnit<string>("FullName");
                v.AddUnit<string>("DerivedAt");
            });

            population.DerivationById["FullName"] = new FullNameDerivation();

            dynamic john = population.New<Person>();
            john.FirstName = "John";
            john.LastName = "Doe";

            population.Derive();

            Assert.Equal("John Doe", john.FullName);

            population.DerivationById["FullName"] = new GreetingDerivation(population.DerivationById["FullName"]);

            dynamic jane = population.New<Person>();
            jane.FirstName = "Jane";
            jane.LastName = "Doe";

            population.Derive();

            Assert.Equal("Jane Doe Chained", jane.FullName);
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
            private readonly IDynamicDerivation derivation;

            public GreetingDerivation(IDynamicDerivation derivation)
            {
                this.derivation = derivation;
            }

            public void Derive(DynamicChangeSet changeSet)
            {
                this.derivation.Derive(changeSet);

                System.Collections.Generic.Dictionary<DynamicObject, object> firstNames = changeSet.ChangedRoles("FirstName");
                System.Collections.Generic.Dictionary<DynamicObject, object> lastNames = changeSet.ChangedRoles("LastName");

                if (firstNames?.Any() == true || lastNames?.Any() == true)
                {
                    System.Collections.Generic.IEnumerable<DynamicObject> people = firstNames.Union(lastNames).Select(v => v.Key).Distinct();

                    foreach (dynamic person in people)
                    {
                        person.FullName = $"{person.FullName} Chained";
                    }
                }
            }
        }
    }
}
