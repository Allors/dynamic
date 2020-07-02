namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicManyToOneAssociationType : DynamicManyToAssociationType
    {
        public DynamicManyToOneAssociationType(DynamicManyToOneRoleType roleType, Type type)
        {
            roleType.AssociationType = this;
            this.RoleType = roleType;
            this.Type = type;
        }

        DynamicRoleType DynamicAssociationType.RoleType => this.RoleType;

        public DynamicManyToOneRoleType RoleType { get; }

        public Type Type { get; }

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
