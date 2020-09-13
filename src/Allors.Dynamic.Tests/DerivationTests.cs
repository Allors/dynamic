namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Meta;
    using Allors.Dynamic.Tests.Domain;
    using System;
    using System.Linq;
    using Xunit;

    public class DerivationTests
    {
        [Fact]
        public void Derivation()
        {
            var population = new Default.DynamicPopulation(
                 new DynamicMeta(new Pluralizer()),
                 v =>
            {
                v.AddUnit<Person, string>("FirstName");
                v.AddUnit<Person, string>("LastName");
                v.AddUnit<Person, string>("FullName");
                v.AddUnit<Person, string>("DerivedAt");
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
                var firstNames = changeSet.ChangedRoles<Person>("FirstName");
                var lastNames = changeSet.ChangedRoles<Person>("LastName");

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

                var firstNames = changeSet.ChangedRoles<Person>("FirstName");
                var lastNames = changeSet.ChangedRoles<Person>("LastName");

                if (firstNames?.Any() == true || lastNames?.Any() == true)
                {
                    var people = firstNames.Union(lastNames).Select(v => v.Key).Distinct();

                    foreach (dynamic person in people)
                    {
                        person.FullName = $"{person.FullName} Chained";
                    }
                }
            }
        }
    }
}
