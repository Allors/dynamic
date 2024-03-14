namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Meta;

    public class Organization : DynamicObject, INamed
    {
        public Organization(DynamicPopulation population, DynamicObjectType objectType)
            : base(population, objectType)
        {
        }
    }
}
