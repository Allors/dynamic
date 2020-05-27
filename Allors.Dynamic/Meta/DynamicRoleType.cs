namespace Allors.Dynamic.Meta
{
    public class DynamicRoleType
    {
        public DynamicAssociationType AssociationType { get; internal set; }

        public string Name => this.IsOne ? this.SingularName : this.PluralName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => !IsMany;

        public bool IsMany { get; internal set; }

        public bool IsUnit => this.AssociationType == null;

        public bool IsComposite => this.AssociationType != null;

        public override string ToString() => this.Name;
    }
}
