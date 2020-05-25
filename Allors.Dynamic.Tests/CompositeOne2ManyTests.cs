using Xunit;

namespace Allors.Dynamic.Tests
{
    public class CompositeOne2ManyTests
    {
        [Fact]
        public void OneToMany()
        {
            var population = new DynamicPopulation(v => v
                 .AddCompositeRelation("Employer", false, "Employee", true)
              );

            dynamic acme = population.NewObject();
            dynamic jane = population.NewObject();
            dynamic john = population.NewObject();
            dynamic jenny = population.NewObject();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            //Assert.Equal(acme, jane.Employer);
            //Assert.Equal(acme, john.Employer);
            //Assert.Equal(acme, jenny.Employer);
        }
    }
}
