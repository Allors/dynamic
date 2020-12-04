namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Meta;

    public class Organization : DynamicObject, INamed
    {
        public Organization(IDynamicPopulation population, DynamicObjectType objectType)
            : base(population, objectType)
        {
        }
    }
}
