using System.Linq;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class CompositeOneToManyTests
    {
        [Fact]
        public void AddSameAssociation()
        {
            var population = new DynamicPopulation(v => v
                    .AddOneToManyRelation("Employer", "Employee")
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

            Assert.Equal(acme, jane.Employer);
            Assert.Equal(acme, john.Employer);
            Assert.Equal(acme, jenny.Employer);
        }


        [Fact]
        public void AddDifferentAssociation()
        {

            var population = new DynamicPopulation(v => v
                 .AddUnitRelation("Name")
                 .AddOneToManyRelation("Employer", "Employee")
              );

            dynamic acme = population.NewObject();

            dynamic jane = population.NewObject();
            jane.Name = "Jane";
            dynamic john = population.NewObject();
            john.Name = "John";
            dynamic jenny = population.NewObject();
            jenny.Name = "Jenny";

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            dynamic hooli = population.NewObject();

            hooli.AddEmployee(jane);

            var people = new[] { jane, john, jenny};

            var x = people.Where(v=>"Jane".Equals(v.FirstName));

            Assert.Contains(jane, hooli.Employees);

            Assert.DoesNotContain(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(hooli, jane.Employer);

            Assert.NotEqual(acme, jane.Employer);
            Assert.Equal(acme, john.Employer);
            Assert.Equal(acme, jenny.Employer);
        }

        [Fact]
        public void Remove()
        {
            var population = new DynamicPopulation(v => v
                 .AddOneToManyRelation("Employer", "Employee")
              );

            dynamic acme = population.NewObject();
            dynamic jane = population.NewObject();
            dynamic john = population.NewObject();
            dynamic jenny = population.NewObject();

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            acme.RemoveEmployee(jane);

            Assert.DoesNotContain(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.NotEqual(acme, jane.Employer);
            Assert.Equal(acme, john.Employer);
            Assert.Equal(acme, jenny.Employer);

            acme.RemoveEmployee(john);

            Assert.DoesNotContain(jane, acme.Employees);
            Assert.DoesNotContain(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.NotEqual(acme, jane.Employer);
            Assert.NotEqual(acme, john.Employer);
            Assert.Equal(acme, jenny.Employer);

            acme.RemoveEmployee(jenny);

            Assert.DoesNotContain(jane, acme.Employees);
            Assert.DoesNotContain(john, acme.Employees);
            Assert.DoesNotContain(jenny, acme.Employees);

            Assert.NotEqual(acme, jane.Employer);
            Assert.NotEqual(acme, john.Employer);
            Assert.NotEqual(acme, jenny.Employer);
        }
    }
}
