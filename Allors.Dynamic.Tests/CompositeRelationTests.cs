using System.Linq;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class CompositeRelationTests
    {
        [Fact]
        public void OneToOne()
        {
            var population = new Population();

            dynamic acme = population.NewObject();
            dynamic gizmo = population.NewObject();
            dynamic jane = population.NewObject();
            dynamic john = population.NewObject();

            acme.Owner = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.WhereOwner);

            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["WhereOwner"]);

            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["WhereOwner"]);
        }

        public void OneToOneWithIndex()
        {
            var population = new Population();

            dynamic acme = population.NewObject();
            dynamic gizmo = population.NewObject();
            dynamic jane = population.NewObject();
            dynamic john = population.NewObject();

            acme["Owner"] = jane;

            Assert.Equal(jane, acme.Owner);
            Assert.Equal(acme, jane.WhereOwner);

            Assert.Equal(jane, acme["Owner"]);
            Assert.Equal(acme, jane["WhereOwner"]);

            Assert.Null(gizmo["Owner"]);
            Assert.Null(john["WhereOwner"]);
        }

        [Fact]
        public void OneToOneDynamicRelations()
        {
            var population = new Population();

            foreach (var run in Enumerable.Range(0, 1000 * 1000))
            {
                dynamic acme = population.NewObject();
                dynamic gizmo = population.NewObject();
                dynamic jane = population.NewObject();
                dynamic john = population.NewObject();

                var role = $"Owner{run}";
                var association = $"WhereOwner{run}";

                acme[role] = jane;

                Assert.Equal(jane, acme[role]);
                Assert.Equal(acme, jane[association]);

                Assert.Null(gizmo[role]);
                Assert.Null(john[association]);
            }
        }

        //[Fact]
        //public void OneToMany()
        //{
        //    var population = new Population();

        //    dynamic acme = population.NewObject();
        //    dynamic jane = population.NewObject();
        //    dynamic john = population.NewObject();
        //    dynamic jenny = population.NewObject();

        //    acme.AddEmployee(jane);
        //    acme.AddEmployee(john);
        //    acme.AddEmployee(jenny);

        //    Assert.Contains(jane, acme.Employees);
        //    Assert.Contains(john, acme.Employees);
        //    Assert.Contains(jenny, acme.Employees);

        //    //Assert.Equal(acme, jane.Employer);
        //    //Assert.Equal(acme, john.Employer);
        //    //Assert.Equal(acme, jenny.Employer);
        //}
    }
}
