namespace Allors.Dynamic.Tests.Domain
{
    public class Person : DynamicObject, Named
    {
        public Person(IDynamicPopulation population)
            : base(population)
        {
        }
    }
}
