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
            this.ObjectType = objectType;
            this.RoleType = roleType;
            this.SingularName = singularName;
            this.PluralName = pluralName;
            this.Name = name;
            this.IsOne = isOne;
            this.IsMany = isMany;
        }
    }
}
