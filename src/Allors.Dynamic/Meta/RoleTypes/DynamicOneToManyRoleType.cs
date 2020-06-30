namespace Allors.Dynamic.Meta
{
    public class DynamicOneToManyRoleType : DynamicToManyRoleType
    {
        public DynamicOneToManyRoleType(DynamicMeta meta)
        {
            this.Meta = meta;
        }

        public DynamicMeta Meta { get; }

        DynamicAssociationType DynamicRoleType.AssociationType => this.AssociationType;

        public DynamicOneToManyAssociationType AssociationType { get; internal set; }

        public string Name => this.PluralName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => false;

        public bool IsMany => true;

        public bool IsUnit => false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

        public void Deconstruct(out DynamicOneToManyAssociationType associationType, out DynamicOneToManyRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }
    }
}