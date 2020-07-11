namespace Allors.Dynamic.Tests.Domain
{
    public class C2: DynamicObject
    {
        public C2(IDynamicPopulation population)
           : base(population)
        {
        }

        public string Same()
        {
            return this.GetRole<string>(nameof(Same));
        }

        public C2 Same(string value)
        {
            this.SetRole(nameof(Same), value);
            return this;
        }

    }
}
