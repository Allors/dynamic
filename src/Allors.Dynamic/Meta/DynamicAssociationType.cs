namespace Allors.Dynamic.Meta
{
    public sealed class DynamicAssociationType
    {
        public DynamicObjectType ObjectType { get; }

        public DynamicRoleType RoleType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne { get; }

        public bool IsMany { get; }

        internal DynamicAssociationType(DynamicObjectType objectType, DynamicRoleType roleType, string singularName, string pluralName, string name, bool isOne, bool isMany)
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
