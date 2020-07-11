namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicManyToManyAssociationType : IDynamicManyToAssociationType
    {
        public DynamicManyToManyAssociationType(DynamicObjectType objectType, DynamicManyToManyRoleType roleType)
        {
            this.ObjectType = objectType;
            roleType.AssociationType = this;
            this.RoleType = roleType;
            this.SingularName = roleType.SingularNameForAssociation(objectType);
            this.PluralName = roleType.PluralNameForAssociation(objectType);
        }

        public DynamicObjectType ObjectType { get; }

        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicManyToManyRoleType RoleType { get; }

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
