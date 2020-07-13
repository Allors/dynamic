namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Meta;

    public class Person : DynamicObject, Named
    {
        public Person(IDynamicPopulation population, DynamicObjectType objectType)
            : base(population, objectType)
        {
        }
    }
}
