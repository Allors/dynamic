namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Meta;

    public class Organisation : DynamicObject, Named
    {
        public Organisation(IDynamicPopulation population, DynamicObjectType objectType)
            : base(population, objectType)
        {
        }
    }
}
