namespace Allors.Dynamic.Meta
{
    public sealed class DynamicUnitAssociationType : IDynamicAssociationType
    {
        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicUnitRoleType RoleType { get; internal set; }

        public DynamicObjectType ObjectType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne { get; }

        public bool IsMany { get; }

        internal DynamicUnitAssociationType(DynamicObjectType objectType, DynamicUnitRoleType roleType, string singularName,
            string pluralName, string name, bool isOne, bool isMany)
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
