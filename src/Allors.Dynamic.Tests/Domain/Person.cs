namespace Allors.Dynamic.Tests.Domain
{
    public class Person : DynamicObject
    {
        public Person(DynamicPopulation population)
            : base(population)
        {
        }
    }
}
