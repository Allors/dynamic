namespace Allors.Dynamic.Meta
{
    public class DynamicAssociationType
    {
        public DynamicRoleType RoleType { get; }

        public string Name { get; set; }

        public bool IsMany { get; set; }

        internal DynamicAssociationType(DynamicRoleType roleType)
        {
            roleType.AssociationType = this;
            RoleType = roleType;
            this.IsMany = true;
        }
    }
}
