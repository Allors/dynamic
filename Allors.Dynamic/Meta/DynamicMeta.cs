using System;
using System.Collections.Generic;
using System.Globalization;

namespace Allors.Dynamic.Meta
{
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

        public DynamicMeta AddUnitRelation(string roleName)
        {
            var roleType = new DynamicRoleType
            {
                SingularName = roleName,
                PluralName = inflector.Pluralize(roleName),
                IsMany = false,
            };

            this.AddRoleType(roleType);

            return this;
        }

        public DynamicMeta AddOneToOneRelation(string associationName, string roleName)
        {
            return this.AddCompositeRelation(associationName, false, roleName, false);
        }

        public DynamicMeta AddOneToManyRelation(string associationName, string roleName)
        {
            return this.AddCompositeRelation(associationName, false, roleName, true);
        }

        public DynamicMeta AddManyToOneRelation(string associationName, string roleName)
        {
            return this.AddCompositeRelation(associationName, true, roleName, false);
        }

        public DynamicMeta AddManyToManyRelation(string associationName, string roleName)
        {
            return this.AddCompositeRelation(associationName, true, roleName, true);
        }

        internal DynamicMeta AddCompositeRelation(string associationName, bool associationIsMany, string roleName, bool roleIsMany)
        {
            var roleType = new DynamicRoleType
            {
                SingularName = roleName,
                PluralName = inflector.Pluralize(roleName),
                IsMany = roleIsMany,
            };

            this.AddRoleType(roleType);

            var associationType = new DynamicAssociationType(roleType)
            {
                SingularName = associationName,
                PluralName = inflector.Pluralize(associationName),
                IsMany = associationIsMany,
            };

            this.AddAssociationType(associationType);

            return this;
        }

        private void AddRoleType(DynamicRoleType roleType)
        {
            this.CheckNames(roleType.SingularName, roleType.PluralName);

            this.RoleTypeByName.Add(roleType.SingularName, roleType);
            this.RoleTypeByName.Add(roleType.PluralName, roleType);
        }

        private void AddAssociationType(DynamicAssociationType associationType)
        {
            this.CheckNames(associationType.SingularName, associationType.PluralName);

            this.AssociationTypeByName.Add(associationType.SingularName, associationType);
            this.AssociationTypeByName.Add(associationType.PluralName, associationType);
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
