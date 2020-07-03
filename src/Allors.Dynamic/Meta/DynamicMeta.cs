namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class DynamicMeta
    {
        internal DynamicMeta(IPluralizer pluralizer)
        {
            this.Pluralizer = pluralizer;
            this.AssociationTypeByName = new Dictionary<string, DynamicAssociationType>();
            this.RoleTypeByName = new Dictionary<string, DynamicRoleType>();
        }

        public IPluralizer Pluralizer { get; }

        public Dictionary<string, DynamicAssociationType> AssociationTypeByName { get; }

        public Dictionary<string, DynamicRoleType> RoleTypeByName { get; }

        public DynamicUnitRoleType AddUnit<TAssociation, TRole>(string roleName)
        {
            var roleType = new DynamicUnitRoleType(this, typeof(TRole), roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicUnitAssociationType(roleType, typeof(TAssociation));
            this.AddAssociationType(associationType);

            return roleType;
        }

        public DynamicOneToOneRoleType AddOneToOne<TAssociation, TRole>(string associationName, string roleName)
        {
            var roleType = new DynamicOneToOneRoleType(this, typeof(TRole), roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicOneToOneAssociationType(roleType, typeof(TAssociation));
            this.AddAssociationType(associationType);

            return roleType;
        }

        public DynamicManyToOneRoleType AddManyToOne<TAssociation, TRole>(string associationName, string roleName)
        {
            var roleType = new DynamicManyToOneRoleType(this, typeof(TRole), roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicManyToOneAssociationType(roleType, typeof(TAssociation));
            this.AddAssociationType(associationType);

            return roleType;
        }

        public DynamicOneToManyRoleType AddOneToMany<TAssociation, TRole>(string associationName, string roleName)
        {
            var roleType = new DynamicOneToManyRoleType(this, typeof(TRole), roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicOneToManyAssociationType(roleType, typeof(TAssociation));
            this.AddAssociationType(associationType);

            return roleType;
        }

        public DynamicManyToManyRoleType AddManyToMany<TAssociation, TRole>(string associationName, string roleName)
        {
            var roleType = new DynamicManyToManyRoleType(this, typeof(TRole), roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicManyToManyAssociationType(roleType, typeof(TAssociation));
            this.AddAssociationType(associationType);

            return roleType;
        }

        private void AddAssociationType(DynamicAssociationType associationType)
        {
            this.CheckNames(associationType.SingularName, associationType.PluralName);

            this.AssociationTypeByName.Add(associationType.SingularName, associationType);
            this.AssociationTypeByName.Add(associationType.PluralName, associationType);
        }

        private void AddRoleType(DynamicRoleType roleType)
        {
            this.CheckNames(roleType.SingularName, roleType.PluralName);

            this.RoleTypeByName.Add(roleType.SingularName, roleType);
            this.RoleTypeByName.Add(roleType.PluralName, roleType);
        }

        private void CheckNames(string singularName, string pluralName)
        {
            if (this.RoleTypeByName.ContainsKey(singularName) ||
                this.AssociationTypeByName.ContainsKey(singularName))
            {
                throw new Exception($"{singularName} is not unique");
            }

            if (this.RoleTypeByName.ContainsKey(pluralName) ||
                this.AssociationTypeByName.ContainsKey(pluralName))
            {
                throw new Exception($"{pluralName} is not unique");
            }
        }
    }
}