namespace Allors.Dynamic.Meta
{
    public sealed class DynamicManyToOneAssociationType : IDynamicManyToAssociationType
    {
        internal DynamicManyToOneAssociationType(DynamicObjectType objectType, DynamicManyToOneRoleType roleType, string singularName, string pluralName, string name)
        {
            this.ObjectType = objectType;
            this.RoleType = roleType;
            this.SingularName = singularName;
            this.PluralName = pluralName;
            this.Name = name;
        }

        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicManyToOneRoleType RoleType { get; }

        public DynamicObjectType ObjectType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne => false;

        public bool IsMany => true;
    }
}
