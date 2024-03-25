namespace Allors.Dynamic.Meta
{
    public sealed class DynamicManyToOneAssociationType : IDynamicManyToAssociationType
    {
        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicManyToOneRoleType RoleType { get; internal set; }

        public DynamicObjectType ObjectType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne { get; }

        public bool IsMany { get; }

        internal DynamicManyToOneAssociationType(DynamicObjectType objectType, DynamicManyToOneRoleType roleType, string singularName, string pluralName, string name, bool isOne, bool isMany)
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
