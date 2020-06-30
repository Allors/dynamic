using System;

namespace Allors.Dynamic.Meta
{
    public class DynamicUnitRoleType : DynamicRoleType
    {
        public DynamicUnitRoleType(DynamicMeta meta, Type type)
        {
            this.Meta = meta;
            this.Type = type;
        }

        public Type Type { get; }

        public DynamicMeta Meta { get; }

        public DynamicAssociationType AssociationType { get; internal set; }

        public string Name => this.SingularName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => true;

        public bool IsMany => false;

        public bool IsUnit => true;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }


    }
}