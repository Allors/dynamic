using System;

namespace Allors.Dynamic.Meta
{
    public class DynamicManyToManyAssociationType : DynamicManyToAssociationType
    {
        public DynamicManyToManyAssociationType(DynamicManyToManyRoleType roleType)
        {
            roleType.AssociationType = this;
            this.RoleType = roleType;
        }

        DynamicRoleType DynamicAssociationType.RoleType => this.RoleType;

        public DynamicManyToManyRoleType RoleType { get; }

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
