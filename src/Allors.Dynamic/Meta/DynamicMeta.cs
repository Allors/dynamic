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

        public DynamicUnitRoleType AddUnit(Type type, string name)
        {
            var roleType = new DynamicUnitRoleType(this, type)
            {
                SingularName = name,
                PluralName = this.inflector.Pluralize(name),
            };

            this.AddRoleType(roleType);

            return roleType;
        }

        public DynamicUnitRoleType AddUnit<TRole>(string name) => this.AddUnit(typeof(TRole), name);

        public DynamicOneToOneRoleType AddOneToOne<TAssociation, TRole>(string associationName, string roleName)
        {
            var roleType = new DynamicOneToOneRoleType(this, typeof(TRole))
            {
                SingularName = roleName,
                PluralName = this.inflector.Pluralize(roleName),
            };

            this.AddRoleType(roleType);

            var associationType = new DynamicOneToOneAssociationType(roleType, typeof(TAssociation))
            {
                SingularName = associationName,
                PluralName = this.inflector.Pluralize(associationName),
            };

            this.AddAssociationType(associationType);

            return roleType;
        }

        public DynamicManyToOneRoleType AddManyToOne<TAssociation, TRole>(string associationName, string roleName)
        {
            var roleType = new DynamicManyToOneRoleType(this, typeof(TRole))
            {
                SingularName = roleName,
                PluralName = this.inflector.Pluralize(roleName),
            };

            this.AddRoleType(roleType);

            var associationType = new DynamicManyToOneAssociationType(roleType, typeof(TAssociation))
            {
                SingularName = associationName,
                PluralName = this.inflector.Pluralize(associationName),
            };

            this.AddAssociationType(associationType);

            return roleType;
        }

        public DynamicOneToManyRoleType AddOneToMany<TAssociation, TRole>(string associationName, string roleName)
        {
            var roleType = new DynamicOneToManyRoleType(this, typeof(TRole))
            {
                SingularName = roleName,
                PluralName = this.inflector.Pluralize(roleName),
            };

            this.AddRoleType(roleType);

            var associationType = new DynamicOneToManyAssociationType(roleType, typeof(TAssociation))
            {
                SingularName = associationName,
                PluralName = this.inflector.Pluralize(associationName),
            };

            this.AddAssociationType(associationType);

            return roleType;
        }

        public DynamicManyToManyRoleType AddManyToMany<TAssociation, TRole>(string associationName, string roleName)
        {
            var roleType = new DynamicManyToManyRoleType(this, typeof(TRole))
            {
                SingularName = roleName,
                PluralName = this.inflector.Pluralize(roleName),
            };

            this.AddRoleType(roleType);

            var associationType = new DynamicManyToManyAssociationType(roleType, typeof(TAssociation))
            {
                SingularName = associationName,
                PluralName = this.inflector.Pluralize(associationName),
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