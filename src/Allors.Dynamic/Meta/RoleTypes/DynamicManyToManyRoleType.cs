namespace Allors.Dynamic.Meta
{
    public class DynamicManyToManyRoleType : DynamicToManyRoleType
    {
        public DynamicManyToManyRoleType(DynamicMeta meta)
        {
            this.Meta = meta;
        }

        public DynamicMeta Meta { get; }

        DynamicAssociationType DynamicRoleType.AssociationType => this.AssociationType;

        public DynamicManyToManyAssociationType AssociationType { get; internal set; }

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

        public void Deconstruct(out DynamicManyToManyAssociationType associationType, out DynamicManyToManyRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }
    }
}