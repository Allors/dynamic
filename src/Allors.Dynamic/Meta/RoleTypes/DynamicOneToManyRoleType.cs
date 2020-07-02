namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public class DynamicOneToManyRoleType : DynamicToManyRoleType
    {
        public DynamicOneToManyRoleType(DynamicMeta meta, Type type)
        {
            this.Meta = meta;
            this.Type = type;
            this.TypeCode = System.Type.GetTypeCode(type);
        }

        public DynamicMeta Meta { get; }

        public Type Type { get; }

        public TypeCode TypeCode { get; }

        DynamicAssociationType DynamicRoleType.AssociationType => this.AssociationType;

        public DynamicOneToManyAssociationType AssociationType { get; internal set; }

        public string Name => this.PluralName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => false;

        public bool IsMany => true;

        public bool IsUnit => false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

        public void Deconstruct(out DynamicOneToManyAssociationType associationType, out DynamicOneToManyRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }

        public object Normalize(object value) => this.NormalizeToMany(value);
    }
}