namespace Allors.Dynamic.Meta
{
    public class DynamicLinkedType
    {
        public DynamicLinkerType LinkerType { get; internal set; }

        public string Name => this.IsOne ? this.SingularName : this.PluralName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => !IsMany;

        public bool IsMany { get; internal set; }

        public bool IsData => this.LinkerType == null;

        public override string ToString() => this.Name;
    }
}