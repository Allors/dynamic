namespace Allors.Dynamic.Meta
{
    public sealed class DynamicOneToOneAssociationType : IDynamicAssociationType
    {
        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicOneToOneRoleType RoleType { get; internal set; }

        public DynamicObjectType ObjectType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne { get; }

        public bool IsMany { get; }

        internal DynamicOneToOneAssociationType(DynamicObjectType objectType, DynamicOneToOneRoleType roleType, string singularName, string pluralName, string name, bool isOne, bool isMany)
        {
            ObjectType = objectType;
            RoleType = roleType;
            SingularName = singularName;
            PluralName = pluralName;
            Name = name;
            IsOne = isOne;
            IsMany = isMany;
        }
    }
}
