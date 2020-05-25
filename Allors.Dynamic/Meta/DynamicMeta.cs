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
                Name = roleName,
                IsMany = false,
            };

            this.AddRoleType(roleType);
            
            return this;
        }

        public DynamicMeta AddCompositeRelation(string associationName, bool associationIsMany, string roleName, bool roleIsMany)
        {
            var roleType = new DynamicRoleType
            {
                Name = roleName,
                IsMany = roleIsMany,
            };

            this.AddRoleType(roleType);

            var associationType = new DynamicAssociationType(roleType)
            {
                Name = associationName,
                IsMany = associationIsMany,
            };

            this.AddAssociationType(associationType);

            return this;
        }

        private void AddRoleType(DynamicRoleType roleType)
        {
            var singularName = roleType.Name;
            var pluralName = inflector.Pluralize(singularName);
            
            this.CheckNames(singularName, pluralName);

            this.RoleTypeByName.Add(singularName,  roleType);
            this.RoleTypeByName.Add(pluralName, roleType);
        }

        private void AddAssociationType(DynamicAssociationType associationType)
        {
            var singularName = associationType.Name;
            var pluralName = inflector.Pluralize(singularName);

            this.CheckNames(singularName, pluralName);

            this.AssociationTypeByName.Add(singularName, associationType);
            this.AssociationTypeByName.Add(pluralName, associationType);
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
