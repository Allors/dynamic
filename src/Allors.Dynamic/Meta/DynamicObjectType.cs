namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DynamicObjectType
    {
        private readonly Dictionary<string, DynamicAssociationType> assignedAssociationTypeByName;

        private readonly Dictionary<string, DynamicRoleType> assignedRoleTypeByName;

        private IDictionary<string, DynamicAssociationType>? derivedAssociationTypeByName;

        private IDictionary<string, DynamicRoleType>? derivedRoleTypeByName;

        internal DynamicObjectType(DynamicMeta dynamicMeta, Type type)
        {
            this.DynamicMeta = dynamicMeta;
            this.Type = type;
            this.TypeCode = Type.GetTypeCode(type);
            this.SuperTypes = new HashSet<DynamicObjectType>();
            this.assignedAssociationTypeByName = new Dictionary<string, DynamicAssociationType>();
            this.assignedRoleTypeByName = new Dictionary<string, DynamicRoleType>();

            this.EmptyArray = Array.CreateInstance(type, 0);

            var hierarchyChanged = false;
            foreach (var other in dynamicMeta.ObjectTypeByType.Values)
            {
                if (this.Type.IsAssignableFrom(other.Type))
                {
                    other.SuperTypes.Add(this);
                    hierarchyChanged = true;
                }

                if (other.Type.IsAssignableFrom(this.Type))
                {
                    this.SuperTypes.Add(other);
                    hierarchyChanged = true;
                }
            }

            if (hierarchyChanged)
            {
                this.DynamicMeta.ResetDerivations();
            }
        }

        public DynamicMeta DynamicMeta { get; }

        public TypeCode TypeCode { get; }

        public ISet<DynamicObjectType> SuperTypes { get; }

        public Type Type { get; }

        public IDictionary<string, DynamicAssociationType> AssociationTypeByName
        {
            get
            {
                if (this.derivedAssociationTypeByName == null)
                {
                    this.derivedAssociationTypeByName = new Dictionary<string, DynamicAssociationType>(this.assignedAssociationTypeByName);
                    foreach (var item in this.SuperTypes.SelectMany(v => v.assignedAssociationTypeByName))
                    {
                        this.derivedAssociationTypeByName[item.Key] = item.Value;
                    }
                }

                return this.derivedAssociationTypeByName;
            }
        }

        public IDictionary<string, DynamicRoleType> RoleTypeByName
        {
            get
            {
                if (this.derivedRoleTypeByName == null)
                {
                    this.derivedRoleTypeByName = new Dictionary<string, DynamicRoleType>(this.assignedRoleTypeByName);
                    foreach (var item in this.SuperTypes.SelectMany(v => v.assignedRoleTypeByName))
                    {
                        this.derivedRoleTypeByName[item.Key] = item.Value;
                    }
                }

                return this.derivedRoleTypeByName;
            }
        }

        internal object EmptyArray { get; }

        internal DynamicRoleType AddUnit(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Type.Name;
            string rolePluralName = this.DynamicMeta.Pluralize(roleSingularName);

            var roleType = new DynamicRoleType
            (
                objectType,
                roleSingularName,
                rolePluralName,
                roleSingularName,
                true,
                false,
                true
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.DynamicMeta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicAssociationType(
                this,
                roleType,
                associationSingularName,
                associationPluralName,
                associationSingularName,
                true,
                false
            );

            this.AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            this.DynamicMeta.ResetDerivations();

            return roleType;
        }

        internal DynamicRoleType AddOneToOne(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Type.Name;
            string rolePluralName = this.DynamicMeta.Pluralize(roleSingularName);

            var roleType = new DynamicRoleType
            (
                objectType,
                roleSingularName,
                rolePluralName,
                roleSingularName,
                true,
                false,
                false
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.DynamicMeta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicAssociationType(
                this,
                roleType,
                associationSingularName,
                associationPluralName,
                associationSingularName,
                true,
                false
            );

            this.AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            this.DynamicMeta.ResetDerivations();

            return roleType;
        }

        internal DynamicRoleType AddManyToOne(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Type.Name;
            string rolePluralName = this.DynamicMeta.Pluralize(roleSingularName);

            var roleType = new DynamicRoleType
            (
                objectType,
                roleSingularName,
                rolePluralName,
                roleSingularName,
                true,
                false,
                false
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.DynamicMeta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicAssociationType
            (
                this,
                roleType,
                associationSingularName,
                associationPluralName,
                associationPluralName,
                false,
                true
            );

            this.AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            this.DynamicMeta.ResetDerivations();

            return roleType;
        }

        internal DynamicRoleType AddOneToMany(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Type.Name;
            string rolePluralName = this.DynamicMeta.Pluralize(roleSingularName);

            var roleType = new DynamicRoleType
            (
                objectType,
                roleSingularName,
                rolePluralName,
                rolePluralName,
                false,
                true,
                false
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.DynamicMeta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicAssociationType(
                this,
                roleType,
                associationSingularName,
                associationPluralName,
                associationSingularName,
                true,
                false
            );

            this.AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            this.DynamicMeta.ResetDerivations();

            return roleType;
        }

        internal DynamicRoleType AddManyToMany(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Type.Name;
            string rolePluralName = this.DynamicMeta.Pluralize(roleSingularName);

            var roleType = new DynamicRoleType(
                objectType,
                roleSingularName,
                rolePluralName,
                rolePluralName,
                false,
                true,
                false
            );
            
            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.DynamicMeta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicAssociationType
            (
                this,
                roleType,
                associationSingularName,
                associationPluralName,
                associationPluralName,
                false,
                true
            );

            this.AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            this.DynamicMeta.ResetDerivations();

            return roleType;
        }

        internal void ResetDerivations()
        {
            this.derivedAssociationTypeByName = null;
            this.derivedRoleTypeByName = null;
        }

        private void AddAssociationType(DynamicAssociationType associationType)
        {
            this.CheckNames(associationType.SingularName, associationType.PluralName);

            this.assignedAssociationTypeByName.Add(associationType.SingularName, associationType);
            this.assignedAssociationTypeByName.Add(associationType.PluralName, associationType);
        }

        private void AddRoleType(DynamicRoleType roleType)
        {
            this.CheckNames(roleType.SingularName, roleType.PluralName);

            this.assignedRoleTypeByName.Add(roleType.SingularName, roleType);
            this.assignedRoleTypeByName.Add(roleType.PluralName, roleType);
        }

        private void CheckNames(string singularName, string pluralName)
        {
            if (this.RoleTypeByName.ContainsKey(singularName) ||
                this.AssociationTypeByName.ContainsKey(singularName))
            {
                throw new ArgumentException($"{singularName} is not unique");
            }

            if (this.RoleTypeByName.ContainsKey(pluralName) ||
                this.AssociationTypeByName.ContainsKey(pluralName))
            {
                throw new ArgumentException($"{pluralName} is not unique");
            }
        }
    }
}
