namespace Allors.Dynamic.Tests.Domain
{
    public class C1 : DynamicObject
    {
        public C1(IDynamicPopulation population)
           : base(population)
        {
        }

        public string Same()
        {
            return this.GetRole<string>(nameof(Same));
        }

        public C1 Same(string value)
        {
            this.SetRole(nameof(Same), value);
            return this;
        }
    }
}
