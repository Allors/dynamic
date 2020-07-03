namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicManyToManyAssociationType : IDynamicManyToAssociationType
    {
        public DynamicManyToManyAssociationType(DynamicManyToManyRoleType roleType, Type type)
        {
            roleType.AssociationType = this;
            this.RoleType = roleType;
            this.Type = type;
            this.SingularName = roleType.SingularNameForAssociation(type);
            this.PluralName = roleType.PluralNameForAssociation(type);
        }

        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicManyToManyRoleType RoleType { get; }

        public Type Type { get; }

        public string Name => this.PluralName;

        public string SingularName { get; }

        public string PluralName { get; }

        public bool IsOne => false;

        public bool IsMany => true;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
