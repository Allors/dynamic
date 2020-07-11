namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicManyToOneAssociationType : IDynamicManyToAssociationType
    {
        public DynamicManyToOneAssociationType(DynamicObjectType objectType, DynamicManyToOneRoleType roleType)
        {
            this.ObjectType = objectType;
            roleType.AssociationType = this;
            this.RoleType = roleType;
            this.SingularName = roleType.SingularNameForAssociation(objectType);
            this.PluralName = roleType.PluralNameForAssociation(objectType);
        }

        public DynamicObjectType ObjectType { get; }

        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicManyToOneRoleType RoleType { get; }

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
