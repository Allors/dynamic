namespace Allors.Dynamic.Meta
{
    public class DynamicLinkerType
    {
        public DynamicLinkedType LinkedType { get; }

        public string Name => this.IsOne ? this.SingularName : this.PluralName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => !this.IsMany;

        public bool IsMany { get; internal set; }

        public DynamicLinkerType(DynamicLinkedType linkedType)
        {
            linkedType.LinkerType = this;
            this.LinkedType = linkedType;
        }

        public override string ToString() => this.Name;
    }
}
