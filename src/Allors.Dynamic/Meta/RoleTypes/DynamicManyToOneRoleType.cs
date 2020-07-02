namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    public class DynamicManyToOneRoleType : DynamicToOneRoleType
    {
        public DynamicManyToOneRoleType(DynamicMeta meta, Type type)
        {
            this.Meta = meta;
            this.Type = type;
            this.TypeCode = System.Type.GetTypeCode(type);
        }

        public DynamicMeta Meta { get; }

        public Type Type { get; }

        public TypeCode TypeCode { get; }

        DynamicAssociationType DynamicRoleType.AssociationType => this.AssociationType;

        public DynamicManyToOneAssociationType AssociationType { get; internal set; }

        public string Name => this.SingularName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

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