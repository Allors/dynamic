namespace Allors.Dynamic.Tests
{
    using Allors.Dynamic.Meta;
    using System;
    using Xunit;

    public class DynamicTests
    {
        public class Organisation
        {
            public Func<dynamic, string> GetName;
            public Func<string, Action<dynamic>> SetName;

            public Func<dynamic, dynamic> GetOwner;
            public Func<dynamic, Action<dynamic>> SetOwner;

            public Func<dynamic, Action<dynamic>> AddEmployee;
            public Func<dynamic, Action<dynamic>> RemoveEmployee;
        }

        public class Person
        {
            public Func<dynamic, string> GetName;
            public Func<string, Action<dynamic>> SetName;

            public Func<dynamic, dynamic> GetProperty;
        }

        [Fact]
        public void Get()
        {
            var population = new DynamicPopulation();

            var name = population.Meta.AddUnit<string>("Name");
            var (property, owner) = population.Meta.AddOneToOne("Property", "Owner");

            var getName = name.Get();
            var setName = name.Set();

            var getOwner = owner.Get();
            var setOwner = owner.Set();
            var getProperty = property.Get();

            var organisation = new Organisation { GetName = getName, SetName = setName, GetOwner = getOwner, SetOwner = setOwner };
            var person = new Person { GetName = getName, SetName = setName, GetProperty = getProperty };

            var newOrganisation = population.Factory(organisation);
            var newPerson = population.Factory(person);

            var jane = newPerson()
                .Apply(o => o.SetName("Jane"));

            var acme = newOrganisation()
                .Apply(
                    o => o.SetName("Acme"),
                    o => o.SetOwner(jane));

            Assert.Equal("Acme", acme[name]);
            Assert.Equal("Jane", jane[name]);

            Assert.Equal("Acme", acme.Apply(v => v.GetName));
            Assert.Equal("Jane", jane.Apply(v => v.GetName));

            Assert.Equal(acme, jane[property]);

            Assert.Equal(acme, jane.Apply(v => v.GetProperty));
        }

        [Fact]
        public void Set()
        {
            var population = new DynamicPopulation();

            var name = population.Meta.AddUnit<string>("Name");
            var (property, owner) = population.Meta.AddOneToOne("Property", "Owner");

            var getName = name.Get();
            var setName = name.Set();
            var setOwner = owner.Set();

            var organisation = new Organisation { GetName = getName, SetName = setName, SetOwner = setOwner };
            var person = new Person { GetName = getName, SetName = setName };

            var newOrganisation = population.Factory(organisation);
            var newPerson = population.Factory(person);

            var jane = newPerson()
                .Apply(o => o.SetName("Jane"));

            var acme = newOrganisation()
                .Apply(
                    o => o.SetName("Acme"),
                    o => o.SetOwner(jane));

            Assert.Equal("Acme", acme.Apply(v => v.GetName));
            Assert.Equal("Jane", jane.Apply(v => v.GetName));

            Assert.Equal(acme, jane[property]);
        }

        [Fact]
        public void Add()
        {
            var population = new DynamicPopulation();

            var name = population.Meta.AddUnit<string>("Name");
            var (employer, employee) = population.Meta.AddOneToMany("Employer", "Employee");

            var setName = name.Set();
            var addEmployee = employee.Add();
            var removeEmployee = employee.Remove();

            var organisation = new Organisation { SetName = setName, AddEmployee = addEmployee, RemoveEmployee = removeEmployee };
            var person = new Person { SetName = setName };

            var newOrganisation = population.Factory(organisation);
            var newPerson = population.Factory(person);

            var acme = newOrganisation(v => v.Apply(o => o.SetName("Acme")));
            var hooli = newOrganisation(v => v.Apply(o => o.SetName("Hooli")));

            var jane = newPerson(v => v.Apply(o => o.SetName("Jane")));
            var john = newPerson(v => v.Apply(o => o.SetName("Jane")));
            var jenny = newPerson(v => v.Apply(o => o.SetName("Jane")));

            var people = new[] { jane, john, jenny };

            acme.Apply(v => v.AddEmployee(jane));
            acme.Apply(v => v.AddEmployee(john));
            acme.Apply(v => v.AddEmployee(jenny));

            foreach (var p in people)
            {
                Assert.Contains(p, acme[employee]);
                Assert.DoesNotContain(p, hooli[employee]);

                Assert.Equal(acme, p[employer]);
            }

            hooli.Apply(v => v.AddEmployee(jane));

            Assert.Contains(jane, hooli[employee]);

            Assert.DoesNotContain(jane, acme[employee]);
            Assert.Contains(john, acme[employee]);
            Assert.Contains(jenny, acme[employee]);

            Assert.Equal(hooli, jane[employer]);

            Assert.NotEqual(acme, jane[employer]);
            Assert.Equal(acme, john[employer]);
            Assert.Equal(acme, jenny[employer]);
        }
    }
}
