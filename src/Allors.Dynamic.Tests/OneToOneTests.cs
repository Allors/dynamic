namespace Allors.Dynamic.Tests
{
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
                v.AddOneToOne("Property", "Owner");
            });

            Action<dynamic> name(string name)
            {
                return (obj) => obj.Name = name;
            }

            dynamic acme = population.New(name("Acme"));
            dynamic gizmo = population.New(name("Gizmo"));

            dynamic jane = population.New(name("Jane"));
            dynamic john = population.New(name("John"));

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
