namespace Allors.Dynamic.Meta
{
    public class DynamicAssociationType
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
