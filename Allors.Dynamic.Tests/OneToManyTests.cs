using System.Linq;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class OneToManyTests
    {
        [Fact]
        public void AddSameAssociation()
        {
            var population = new DynamicPopulation(v => v
                    .AddOneToManyAssociation("Employer", "Employee")
              );

            dynamic acme = population.Create();
            dynamic jane = population.Create();
            dynamic john = population.Create();
            dynamic jenny = population.Create();

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
                 .AddDataAssociation("Name")
                 .AddOneToManyAssociation("Employer", "Employee")
              );

            dynamic acme = population.Create();

            dynamic jane = population.Create();
            jane.Name = "Jane";
            dynamic john = population.Create();
            john.Name = "John";
            dynamic jenny = population.Create();
            jenny.Name = "Jenny";

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            dynamic hooli = population.Create();

            hooli.AddEmployee(jane);

            var people = new[] { jane, john, jenny };

            var x = people.Where(v => "Jane".Equals(v.FirstName));

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
                 .AddOneToManyAssociation("Employer", "Employee")
              );

            dynamic acme = population.Create();
            dynamic jane = population.Create();
            dynamic john = population.Create();
            dynamic jenny = population.Create();

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
