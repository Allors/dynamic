
namespace Allors.Dynamic.Meta
{
    public sealed class DynamicManyToOneRoleType : IDynamicToOneRoleType
    {
        IDynamicAssociationType IDynamicRoleType.AssociationType => this.AssociationType;

        public DynamicManyToOneAssociationType AssociationType { get; internal set; } = null!;

        public DynamicObjectType ObjectType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne { get; }

        public bool IsMany { get; }

        void IDynamicRoleType.Deconstruct(out IDynamicRoleType roleType, out IDynamicAssociationType associationType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }

        public void Deconstruct(out DynamicManyToOneRoleType roleType, out DynamicManyToOneAssociationType associationType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }

        internal DynamicManyToOneRoleType(DynamicObjectType objectType, string singularName, string pluralName, string name, bool isOne, bool isMany)
        {
            this.ObjectType = objectType;
            this.SingularName = singularName;
            this.PluralName = pluralName;
            this.Name = name;
            this.IsOne = isOne;
            this.IsMany = isMany;
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal string SingularNameForEmbeddedAssociationType(DynamicObjectType dynamicObjectType)
        {
            return $"{dynamicObjectType.Name}Where{this.SingularName}";
        }

        internal string PluralNameForEmbeddedAssociationType(DynamicObjectType dynamicObjectType)
        {
            return $"{this.ObjectType.Meta.Pluralize(dynamicObjectType.Name)}Where{this.SingularName}";
        }
    }
}
