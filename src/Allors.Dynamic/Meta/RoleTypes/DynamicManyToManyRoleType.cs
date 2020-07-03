namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicManyToManyRoleType : DynamicToManyRoleType
    {
        public DynamicManyToManyRoleType(DynamicMeta meta, Type type, string singularName)
        {
            this.Meta = meta;
            this.Type = type;
            this.TypeCode = Type.GetTypeCode(type);
            this.SingularName = singularName ?? type.Name;
            this.PluralName = meta.Pluralizer.Pluralize(this.SingularName);
        }

        public DynamicMeta Meta { get; }

        public Type Type { get; }

        public TypeCode TypeCode { get; }

        DynamicAssociationType DynamicRoleType.AssociationType => this.AssociationType;

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