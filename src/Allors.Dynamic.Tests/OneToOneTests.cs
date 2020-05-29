namespace Allors.Dynamic.Tests
{
    using System;
    using Xunit;

    public class OneToOneTests
    {
        [Fact]
        public void Set()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                .AddUnitRelationType("Name")
                .AddOneToOneRelationType("Property", "Owner")
             );

            Action<dynamic> name(string name)
            {
                return (obj) => obj.Name = name;
            }

            dynamic acme = population.Create(name("Acme"));
            dynamic gizmo = population.Create(name("Gizmo"));

            dynamic jane = population.Create(name("Jane"));
            dynamic john = population.Create(name("John"));

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
