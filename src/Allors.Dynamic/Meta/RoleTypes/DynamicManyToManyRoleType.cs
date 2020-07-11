namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicManyToManyRoleType : IDynamicToManyRoleType
    {
        public DynamicManyToManyRoleType(DynamicObjectType objectType, string singularName)
        {
            var pluralizer = objectType.Meta.Pluralizer;

            this.ObjectType = objectType;
            this.SingularName = singularName ?? objectType.Type.Name;
            this.PluralName = pluralizer.Pluralize(this.SingularName);
        }

        public DynamicObjectType ObjectType { get; }

        IDynamicAssociationType IDynamicRoleType.AssociationType => this.AssociationType;

        public DynamicManyToManyAssociationType AssociationType { get; internal set; }

        public string Name => this.PluralName;

        public string SingularName { get; }

        public string PluralName { get; }

        public bool IsOne => false;

        public bool IsMany => true;

        public bool IsUnit => false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

        public void Deconstruct(out DynamicManyToManyAssociationType associationType, out DynamicManyToManyRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }

        public object Normalize(object value) => this.NormalizeToMany(value);
    }
}