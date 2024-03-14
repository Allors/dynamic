namespace Allors.Dynamic.Tests.Domain
{
    using Allors.Dynamic.Meta;

    public class C1 : DynamicObject
    {
        public C1(DynamicPopulation population, DynamicObjectType objectType)
           : base(population, objectType)
        {
        }

        public string Same() => (string)this.GetRole(nameof(this.Same));

        public C1 Same(string value)
        {
            this.SetRole(nameof(this.Same), value);
            return this;
        }
    }
}
