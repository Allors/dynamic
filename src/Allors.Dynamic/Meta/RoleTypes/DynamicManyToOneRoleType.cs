namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicManyToOneRoleType : DynamicToOneRoleType
    {
        public DynamicManyToOneRoleType(DynamicMeta meta, Type type, string singularName)
        {
            this.Meta = meta;
            this.Type = type;
            this.TypeCode = System.Type.GetTypeCode(type);
            this.SingularName = singularName ?? type.Name;
            this.PluralName = meta.Pluralizer.Pluralize(this.SingularName);
        }

        public DynamicMeta Meta { get; }

        public Type Type { get; }

        public TypeCode TypeCode { get; }

        DynamicAssociationType DynamicRoleType.AssociationType => this.AssociationType;

        public DynamicManyToOneAssociationType AssociationType { get; internal set; }

        public string Name => this.SingularName;

        public string SingularName { get; }

        public string PluralName { get; }

        public bool IsOne => true;

        public bool IsMany => false;

        public bool IsUnit => false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

        public void Deconstruct(out DynamicManyToOneAssociationType associationType, out DynamicManyToOneRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }

        public object Normalize(object value) => this.NormalizeToOne(value);
    }
}