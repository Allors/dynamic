using System;
using Allors.Dynamic.Meta;
using Xunit;

namespace Allors.Dynamic.Binding.Tests
{
    public class ManyToManyTests
    {
        [Fact]
        public void AddSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization, v => v.Name = "Acme");
            var hooli = population.Create(organization, v => v.Name = "Hooli");

            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Single(jenny.OrganizationWhereEmployee);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Empty(hooli.Employees);
        }

        [Fact]
        public void SetSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization, v => v.Name = "Acme");
            var hooli = population.Create(organization, v => v.Name = "Hooli");

            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.Employees = new[] { jane };

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Empty(john.OrganizationWhereEmployee);

            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Equal(1, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.Employees = new[] { jane, john };

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Equal(2, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.Employees = new[] { jane, john, jenny };

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Single(jenny.OrganizationWhereEmployee);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.Employees = Array.Empty<dynamic>();

            Assert.Empty(jane.OrganizationWhereEmployee);
            Assert.Empty(john.OrganizationWhereEmployee);
            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Empty(acme.Employees);
            Assert.Empty(hooli.Employees);
        }

        [Fact]
        public void RemoveSingleActiveLink()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization, v => v.Name = "Acme");
            var hooli = population.Create(organization, v => v.Name = ("Hooli"));

            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.Employees = new[] { jane, john, jenny };

            acme.RemoveEmployee(jenny);

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Equal(2, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.RemoveEmployee(john);

            Assert.Single(jane.OrganizationWhereEmployee);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);

            Assert.Empty(john.OrganizationWhereEmployee);

            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Equal(1, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);

            Assert.Empty(hooli.Employees);

            acme.RemoveEmployee(jane);

            Assert.Empty(jane.OrganizationWhereEmployee);
            Assert.Empty(john.OrganizationWhereEmployee);
            Assert.Empty(jenny.OrganizationWhereEmployee);

            Assert.Empty(acme.Employees);
            Assert.Empty(hooli.Employees);
        }

        [Fact]
        public void MultipeleActiveLinks()
        {
            var meta = new DynamicMeta();
            var organization = meta.AddClass("Organization");
            var person = meta.AddClass("Person");
            meta.AddUnit<string>(organization, "Name");
            meta.AddManyToMany(organization, person, "Employee");

            var population = new DynamicPopulation(meta);

            var acme = population.Create(organization, v => v.Name = "Acme");
            var hooli = population.Create(organization, v => v.Name = "Hooli");

            var jane = population.Create(person);
            var john = population.Create(person);
            var jenny = population.Create(person);

            acme.AddEmployee(jane);
            acme.AddEmployee(john);
            acme.AddEmployee(jenny);

            hooli.AddEmployee(jane);

            Assert.Equal(2, jane.OrganizationWhereEmployee.Count);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);
            Assert.Contains(hooli, jane.OrganizationWhereEmployee);

            Assert.Single(john.OrganizationWhereEmployee);
            Assert.Contains(acme, john.OrganizationWhereEmployee);

            Assert.Single(jenny.OrganizationWhereEmployee);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Single(hooli.Employees);
            Assert.Contains(jane, hooli.Employees);

            hooli.AddEmployee(john);

            Assert.Equal(2, jane.OrganizationWhereEmployee.Count);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);
            Assert.Contains(hooli, jane.OrganizationWhereEmployee);

            Assert.Equal(2, john.OrganizationWhereEmployee.Count);
            Assert.Contains(acme, john.OrganizationWhereEmployee);
            Assert.Contains(hooli, john.OrganizationWhereEmployee);

            Assert.Single(jenny.OrganizationWhereEmployee);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(2, hooli.Employees.Count);
            Assert.Contains(jane, hooli.Employees);
            Assert.Contains(john, hooli.Employees);

            hooli.AddEmployee(jenny);

            Assert.Equal(2, jane.OrganizationWhereEmployee.Count);
            Assert.Contains(acme, jane.OrganizationWhereEmployee);
            Assert.Contains(hooli, jane.OrganizationWhereEmployee);

            Assert.Equal(2, john.OrganizationWhereEmployee.Count);
            Assert.Contains(acme, john.OrganizationWhereEmployee);
            Assert.Contains(hooli, john.OrganizationWhereEmployee);

            Assert.Equal(2, jenny.OrganizationWhereEmployee.Count);
            Assert.Contains(acme, jenny.OrganizationWhereEmployee);
            Assert.Contains(hooli, jenny.OrganizationWhereEmployee);

            Assert.Equal(3, acme.Employees.Count);
            Assert.Contains(jane, acme.Employees);
            Assert.Contains(john, acme.Employees);
            Assert.Contains(jenny, acme.Employees);

            Assert.Equal(3, hooli.Employees.Count);
            Assert.Contains(jane, hooli.Employees);
            Assert.Contains(john, hooli.Employees);
            Assert.Contains(jenny, hooli.Employees);
        }
    }
}