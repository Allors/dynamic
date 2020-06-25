using System;

namespace Allors.Dynamic.Meta
{
    public class DynamicRoleType
    {
        public DynamicRoleType(DynamicMeta meta)
        {
            this.Meta = meta;
        }

        public DynamicMeta Meta { get; }

        public DynamicAssociationType AssociationType { get; internal set; }

        public string Name => this.IsOne ? this.SingularName : this.PluralName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => !this.IsMany;

        public bool IsMany { get; internal set; }

        public bool IsUnit => this.AssociationType == null;

        public void Deconstruct(out DynamicAssociationType associationType, out DynamicRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}