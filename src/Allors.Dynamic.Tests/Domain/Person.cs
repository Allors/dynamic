namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Meta;

    public class Person : DynamicObject, INamed
    {
        public Person(DynamicPopulation population, DynamicObjectType objectType)
            : base(population, objectType)
        {
        }
    }
}
