namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Meta;

    public class C2: DynamicObject
    {
        public C2(IDynamicPopulation population, DynamicObjectType objectType)
           : base(population, objectType)
        {
        }

        public string Same() => (string)this.GetRole(nameof(Same));

        public C2 Same(string value)
        {
            this.SetRole(nameof(Same), value);
            return this;
        }

    }
}
