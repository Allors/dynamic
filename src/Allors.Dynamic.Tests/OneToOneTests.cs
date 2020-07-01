namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Tests.Domain;
    using System;
    using Xunit;

    public class OneToOneTests
    {
        [Fact]
        public void Set()
        {
            DynamicPopulation population = new DynamicPopulation(v =>
            {
                v.AddUnit<string>("Name");
                v.AddOneToOne<Organisation, Person>("Property", "Owner");
            });

            Action<dynamic> name(string name)
            {
                return (obj) => obj.Name = name;
            }

            dynamic acme = population.New<Organisation>(name("Acme"));
            dynamic gizmo = population.New<Organisation>(name("Gizmo"));

            dynamic jane = population.New<Person>(name("Jane"));
            dynamic john = population.New<Person>(name("John"));

            acme.Owner = jane;
            gizmo.Owner = john;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(john, gizmo.Owner);

            Assert.Equal(acme, jane.Property);
            Assert.Equal(gizmo, john.Property);

            gizmo.Owner = jane;

            Assert.Null(acme.Owner);
            Assert.Equal(jane, gizmo.Owner);

            Assert.Equal(gizmo, jane.Property);
            Assert.Null(john.Property);
        }
    }
}
