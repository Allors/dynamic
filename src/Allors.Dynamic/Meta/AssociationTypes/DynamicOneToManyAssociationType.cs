namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicOneToManyAssociationType : IDynamicOneToAssociationType
    {
        public DynamicOneToManyAssociationType(DynamicObjectType objectType, DynamicOneToManyRoleType roleType)
        {
            this.ObjectType = objectType;
            roleType.AssociationType = this;
            this.RoleType = roleType;
            this.SingularName = roleType.SingularNameForAssociation(objectType);
            this.PluralName = roleType.PluralNameForAssociation(objectType);
        }

        public DynamicObjectType ObjectType { get; }

        IDynamicRoleType IDynamicAssociationType.RoleType => this.RoleType;

        public DynamicOneToManyRoleType RoleType { get; }

        public string Name => this.SingularName;

        public string SingularName { get; }

        public string PluralName { get; }

        public bool IsOne => true;

        public bool IsMany => false;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }
    }
}
