namespace Allors.Dynamic.Meta
{
    using System.Runtime.InteropServices.Marshalling;

    public sealed class DynamicOneToOneAssociationType : IDynamicOneToAssociationType
    {
        internal DynamicOneToOneAssociationType(DynamicObjectType objectType, DynamicOneToOneRoleType roleType, string singularName, string pluralName, string name)
        {
            this.ObjectType = objectType;
            this.RoleType = roleType;
            this.SingularName = singularName;
            this.PluralName = pluralName;
            this.Name = name;
        }

        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicOneToOneRoleType RoleType { get; }

        public DynamicObjectType ObjectType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne => true;

        public bool IsMany => false;
    }
}
