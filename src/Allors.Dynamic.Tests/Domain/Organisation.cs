namespace Allors.Dynamic.Tests.Domain
{
    public class Organisation : DynamicObject, Named
    {
        public Organisation(IDynamicPopulation population)
            : base(population)
        {
        }
    }
}
