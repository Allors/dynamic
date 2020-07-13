namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Meta;

    public class C1 : DynamicObject
    {
        public C1(IDynamicPopulation population, DynamicObjectType objectType)
           : base(population, objectType)
        {
        }

        public string Same() => (string)this.GetRole(nameof(Same));

        public C1 Same(string value)
        {
            this.SetRole(nameof(Same), value);
            return this;
        }
    }
}
