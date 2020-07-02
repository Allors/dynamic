namespace Allors.Dynamic.Tests.Domain
{
    public class Person : DynamicObject, Named
    {
        public Person(DynamicPopulation population)
            : base(population)
        {
        }
    }
}
