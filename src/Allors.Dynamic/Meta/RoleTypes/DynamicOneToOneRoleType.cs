namespace Allors.Dynamic.Meta
{
    public class DynamicOneToOneRoleType : DynamicToOneRoleType
    {
        public DynamicOneToOneRoleType(DynamicMeta meta)
        {
            this.Meta = meta;
        }

        public DynamicMeta Meta { get; }

        DynamicAssociationType DynamicRoleType.AssociationType => this.AssociationType;

        public DynamicOneToOneAssociationType AssociationType { get; internal set; }

        public string Name => this.SingularName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => true;

        public bool IsMany => false;

        public bool IsUnit => false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

        public void Deconstruct(out DynamicOneToOneAssociationType associationType, out DynamicOneToOneRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }
    }
}