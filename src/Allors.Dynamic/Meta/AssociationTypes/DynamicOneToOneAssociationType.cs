namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicOneToOneAssociationType : DynamicOneToAssociationType
    {
        public DynamicOneToOneAssociationType(DynamicOneToOneRoleType roleType, Type type)
        {
            roleType.AssociationType = this;
            this.RoleType = roleType;
            this.Type = type;
        }

        DynamicRoleType DynamicAssociationType.RoleType => this.RoleType;

        public DynamicOneToOneRoleType RoleType { get; }

        public Type Type { get; }

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
