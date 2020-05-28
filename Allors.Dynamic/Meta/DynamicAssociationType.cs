namespace Allors.Dynamic.Meta
{
    public class DynamicAssociationType
    {
        public DynamicRoleType RoleType { get; }

        public string Name => this.IsOne ? this.SingularName : this.PluralName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => !this.IsMany;

        public bool IsMany { get; internal set; }

        public DynamicAssociationType(DynamicRoleType passiveAssociationEnd)
        {
            passiveAssociationEnd.AssociationType = this;
            this.RoleType = passiveAssociationEnd;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
