namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class DynamicMeta
    {
        private readonly Inflector.Inflector inflector;

        internal DynamicMeta()
        {
            this.AssociationTypeByName = new Dictionary<string, DynamicAssociationType>();
            this.RoleTypeByName = new Dictionary<string, DynamicRoleType>();

            this.inflector = new Inflector.Inflector(new CultureInfo("en"));
        }

        public Dictionary<string, DynamicAssociationType> AssociationTypeByName { get; }

        public Dictionary<string, DynamicRoleType> RoleTypeByName { get; }

        public DynamicRoleType AddUnit(string name)
        {
            DynamicRoleType roleType = new DynamicRoleType
            {
                SingularName = name,
                PluralName = this.inflector.Pluralize(name),
                IsMany = false,
            };

            this.AddRoleType(roleType);

            return roleType;
        }

        public DynamicRoleType AddOneToOne(string associationName, string roleName)
        {
            return this.AddRelationType(associationName, false, roleName, false);
        }

        public DynamicRoleType AddOneToMany(string associationName, string roleName)
        {
            return this.AddRelationType(associationName, false, roleName, true);
        }

        public DynamicRoleType AddManyToOne(string associationName, string roleName)
        {
            return this.AddRelationType(associationName, true, roleName, false);
        }

        public DynamicRoleType AddManyToMany(string associationName, string roleName)
        {
            return this.AddRelationType(associationName, true, roleName, true);
        }

        private DynamicRoleType AddRelationType(string associationName, bool associationIsMany, string roleName, bool roleIsMany)
        {
            DynamicRoleType roleType = new DynamicRoleType
            {
                SingularName = roleName,
                PluralName = this.inflector.Pluralize(roleName),
                IsMany = roleIsMany,
            };

            this.AddRoleType(roleType);

            DynamicAssociationType associationType = new DynamicAssociationType(roleType)
            {
                SingularName = associationName,
                PluralName = this.inflector.Pluralize(associationName),
                IsMany = associationIsMany,
            };

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