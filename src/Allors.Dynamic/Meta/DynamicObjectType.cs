using System;
using System.Collections.Generic;
using System.Linq;

namespace Allors.Dynamic.Meta
{
    public sealed class DynamicObjectType
    {
        private readonly Dictionary<string, DynamicAssociationType> assignedAssociationTypeByName;
        private readonly Dictionary<string, DynamicRoleType> assignedRoleTypeByName;
        private readonly HashSet<DynamicObjectType> supertypes;

        private IDictionary<string, DynamicAssociationType>? derivedAssociationTypeByName;
        private IDictionary<string, DynamicRoleType>? derivedRoleTypeByName;
        private HashSet<DynamicObjectType> derivedSupertypes;

        internal DynamicObjectType(DynamicMeta meta, DynamicObjectTypeKind kind, string name, DynamicObjectType[] supertypes)
        {
            Meta = meta;
            Kind = kind;
            Name = name;

            this.supertypes = [..supertypes];
            assignedAssociationTypeByName = [];
            assignedRoleTypeByName = [];

            this.Meta.ResetDerivations();
        }

        internal DynamicObjectType(DynamicMeta meta, Type type) : this(meta, DynamicObjectTypeKind.Unit, type.Name, [])
        {
            Type = type;
            TypeCode = Type.GetTypeCode(type);
        }

        public DynamicMeta Meta { get; }

        public DynamicObjectTypeKind Kind { get; set; }

        public string Name { get; }

        public TypeCode TypeCode { get; }

        public Type Type { get; }

        public IReadOnlySet<DynamicObjectType> Supertypes
        {
            get
            {
                if (this.derivedSupertypes == null)
                {
                    this.derivedSupertypes = [];
                    this.AddSupertypes(this.derivedSupertypes);
                }

                return this.derivedSupertypes;
            }
        }

        private void AddSupertypes(HashSet<DynamicObjectType> newDerivedSupertypes)
        {
            foreach (var supertype in this.supertypes.Where(supertype => !newDerivedSupertypes.Contains(supertype)))
            {
                newDerivedSupertypes.Add(supertype);
                supertype.AddSupertypes(newDerivedSupertypes);
            }
        }

        public void AddSupertype(DynamicObjectType supertype)
        {
            this.supertypes.Add(supertype);
            this.Meta.ResetDerivations();
        }

        public IDictionary<string, DynamicAssociationType> AssociationTypeByName
        {
            get
            {
                if (derivedAssociationTypeByName == null)
                {
                    derivedAssociationTypeByName = new Dictionary<string, DynamicAssociationType>(assignedAssociationTypeByName);
                    foreach (var item in Supertypes.SelectMany(v => v.assignedAssociationTypeByName))
                    {
                        derivedAssociationTypeByName[item.Key] = item.Value;
                    }
                }

                return derivedAssociationTypeByName;
            }
        }

        public IDictionary<string, DynamicRoleType> RoleTypeByName
        {
            get
            {
                if (derivedRoleTypeByName == null)
                {
                    derivedRoleTypeByName = new Dictionary<string, DynamicRoleType>(assignedRoleTypeByName);
                    foreach (var item in Supertypes.SelectMany(v => v.assignedRoleTypeByName))
                    {
                        derivedRoleTypeByName[item.Key] = item.Value;
                    }
                }

                return derivedRoleTypeByName;
            }
        }

        internal DynamicRoleType AddUnit(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = Meta.Pluralize(roleSingularName);

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
                associationPluralName = Meta.Pluralize(associationSingularName);
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

            AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            Meta.ResetDerivations();

            return roleType;
        }

        internal DynamicRoleType AddOneToOne(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = Meta.Pluralize(roleSingularName);

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
                associationPluralName = Meta.Pluralize(associationSingularName);
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

            AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            Meta.ResetDerivations();

            return roleType;
        }

        internal DynamicRoleType AddManyToOne(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = Meta.Pluralize(roleSingularName);

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
                associationPluralName = Meta.Pluralize(associationSingularName);
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

            AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            Meta.ResetDerivations();

            return roleType;
        }

        internal DynamicRoleType AddOneToMany(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = Meta.Pluralize(roleSingularName);

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
                associationPluralName = Meta.Pluralize(associationSingularName);
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

            AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            Meta.ResetDerivations();

            return roleType;
        }

        internal DynamicRoleType AddManyToMany(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = Meta.Pluralize(roleSingularName);

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
                associationPluralName = Meta.Pluralize(associationSingularName);
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

            AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            Meta.ResetDerivations();

            return roleType;
        }

        internal void ResetDerivations()
        {
            derivedSupertypes = null;
            derivedAssociationTypeByName = null;
            derivedRoleTypeByName = null;
        }

        private void AddAssociationType(DynamicAssociationType associationType)
        {
            CheckNames(associationType.SingularName, associationType.PluralName);

            assignedAssociationTypeByName.Add(associationType.SingularName, associationType);
            assignedAssociationTypeByName.Add(associationType.PluralName, associationType);
        }

        private void AddRoleType(DynamicRoleType roleType)
        {
            CheckNames(roleType.SingularName, roleType.PluralName);

            assignedRoleTypeByName.Add(roleType.SingularName, roleType);
            assignedRoleTypeByName.Add(roleType.PluralName, roleType);
        }

        private void CheckNames(string singularName, string pluralName)
        {
            if (RoleTypeByName.ContainsKey(singularName) ||
                AssociationTypeByName.ContainsKey(singularName))
            {
                throw new ArgumentException($"{singularName} is not unique");
            }

            if (RoleTypeByName.ContainsKey(pluralName) ||
                AssociationTypeByName.ContainsKey(pluralName))
            {
                throw new ArgumentException($"{pluralName} is not unique");
            }
        }

        public bool IsAssignableFrom(DynamicObjectType other)
        {
            return this == other || other.Supertypes.Contains(this);
        }
    }
}
