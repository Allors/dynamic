namespace Allors.Dynamic.Meta
{
    public sealed class DynamicOneToManyAssociationType : IDynamicOneToAssociationType
    {
        internal DynamicOneToManyAssociationType(DynamicObjectType objectType, DynamicOneToManyRoleType roleType, string singularName, string pluralName, string name)
        {
            this.ObjectType = objectType;
            this.RoleType = roleType;
            this.SingularName = singularName;
            this.PluralName = pluralName;
            this.Name = name;
        }

        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicOneToManyRoleType RoleType { get; }

        public DynamicObjectType ObjectType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne => true;

        public bool IsMany => false;
    }
}
