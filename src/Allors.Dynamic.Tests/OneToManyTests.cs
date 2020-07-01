namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Tests.Domain;
    using System.Linq;
    using Xunit;

    public class OneToManyTests
    {
        [Fact]
        public void AddSameAssociation()
        {
            DynamicPopulation population = new DynamicPopulation(v => v
                    .AddOneToMany<Organisation, Person>("Employer", "Employee"));

            dynamic acme = population.New<Organisation>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

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
            DynamicPopulation population = new DynamicPopulation(v =>
            {
                v.AddUnit<string>("Name");
                v.AddOneToMany<Organisation, Person>("Employer", "Employee");
            });

            dynamic acme = population.New<Organisation>();

            dynamic jane = population.New<Person>();
            jane.Name = "Jane";
            dynamic john = population.New<Person>();
            john.Name = "John";
            dynamic jenny = population.New<Person>();
            jenny.Name = "Jenny";

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            dynamic hooli = population.New<Organisation>();

            hooli.AddEmployee(jane);

            dynamic[] people = new[] { jane, john, jenny };

            System.Collections.Generic.IEnumerable<dynamic> x = people.Where(v => "Jane".Equals(v.FirstName));

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
            DynamicPopulation population = new DynamicPopulation(v => v
                 .AddOneToMany<Organisation, Person>("Employer", "Employee"));

            dynamic acme = population.New<Organisation>();
            dynamic jane = population.New<Person>();
            dynamic john = population.New<Person>();
            dynamic jenny = population.New<Person>();

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
