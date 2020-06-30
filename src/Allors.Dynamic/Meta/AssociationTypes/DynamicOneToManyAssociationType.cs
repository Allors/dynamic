using System;

namespace Allors.Dynamic.Meta
{
    public class DynamicOneToManyAssociationType : DynamicOneToAssociationType
    {
        public DynamicOneToManyAssociationType(DynamicOneToManyRoleType roleType)
        {
            roleType.AssociationType = this;
            this.RoleType = roleType;
        }

        DynamicRoleType DynamicAssociationType.RoleType => this.RoleType;

        public DynamicOneToManyRoleType RoleType { get; }

        public string Name => this.SingularName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => true;

        public bool IsMany => false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
