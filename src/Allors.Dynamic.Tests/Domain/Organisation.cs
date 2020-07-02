namespace Allors.Dynamic.Tests.Domain
{
    public class Organisation : DynamicObject, Named
    {
        public Organisation(DynamicPopulation population)
            : base(population)
        {
        }
    }
}
