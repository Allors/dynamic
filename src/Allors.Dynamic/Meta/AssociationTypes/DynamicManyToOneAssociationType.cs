using System;

namespace Allors.Dynamic.Meta
{
    public class DynamicManyToOneAssociationType : DynamicManyToAssociationType
    {
        public DynamicManyToOneAssociationType(DynamicManyToOneRoleType roleType)
        {
            roleType.AssociationType = this;
            this.RoleType = roleType;
        }

        DynamicRoleType DynamicAssociationType.RoleType => this.RoleType;

        public DynamicManyToOneRoleType RoleType { get; }

        public string Name => this.PluralName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => false;

        public bool IsMany => true;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
