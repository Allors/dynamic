﻿namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicOneToManyAssociationType : DynamicOneToAssociationType
    {
        public DynamicOneToManyAssociationType(DynamicOneToManyRoleType roleType, Type type)
        {
            roleType.AssociationType = this;
            this.RoleType = roleType;
            this.Type = type;
            this.SingularName = roleType.SingularNameForAssociation(type);
            this.PluralName = roleType.PluralNameForAssociation(type);
        }

        DynamicRoleType DynamicAssociationType.RoleType => this.RoleType;

        public DynamicOneToManyRoleType RoleType { get; }

        public Type Type { get; }

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
