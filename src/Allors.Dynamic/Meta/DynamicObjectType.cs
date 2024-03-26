namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class DynamicObjectType
    {
        private readonly Dictionary<string, IDynamicAssociationType> assignedAssociationTypeByName;
        private readonly Dictionary<string, IDynamicRoleType> assignedRoleTypeByName;
        private readonly HashSet<DynamicObjectType> supertypes;

        private IDictionary<string, IDynamicAssociationType>? derivedAssociationTypeByName;
        private IDictionary<string, IDynamicRoleType>? derivedRoleTypeByName;
        private HashSet<DynamicObjectType>? derivedSupertypes;

        internal DynamicObjectType(DynamicMeta meta, DynamicObjectTypeKind kind, string name, DynamicObjectType[] supertypes)
        {
            this.Meta = meta;
            this.Kind = kind;
            this.Name = name;

            this.supertypes = [.. supertypes];
            this.assignedAssociationTypeByName = [];
            this.assignedRoleTypeByName = [];

            this.Meta.ResetDerivations();
        }

        internal DynamicObjectType(DynamicMeta meta, Type type)
            : this(meta, DynamicObjectTypeKind.Unit, type.Name, [])
        {
            this.Type = type;
            this.TypeCode = Type.GetTypeCode(type);
        }

        public DynamicMeta Meta { get; }

        public DynamicObjectTypeKind Kind { get; set; }

        public string Name { get; }

        public TypeCode? TypeCode { get; }

        public Type? Type { get; }

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

        public IDictionary<string, IDynamicAssociationType> AssociationTypeByName
        {
            get
            {
                if (this.derivedAssociationTypeByName == null)
                {
                    this.derivedAssociationTypeByName = new Dictionary<string, IDynamicAssociationType>(this.assignedAssociationTypeByName);
                    foreach (var item in this.Supertypes.SelectMany(v => v.assignedAssociationTypeByName))
                    {
                        this.derivedAssociationTypeByName[item.Key] = item.Value;
                    }
                }

                return this.derivedAssociationTypeByName;
            }
        }

        public IDictionary<string, IDynamicRoleType> RoleTypeByName
        {
            get
            {
                if (this.derivedRoleTypeByName == null)
                {
                    this.derivedRoleTypeByName = new Dictionary<string, IDynamicRoleType>(this.assignedRoleTypeByName);
                    foreach (var item in this.Supertypes.SelectMany(v => v.assignedRoleTypeByName))
                    {
                        this.derivedRoleTypeByName[item.Key] = item.Value;
                    }
                }

                return this.derivedRoleTypeByName;
            }
        }

        internal DynamicUnitRoleType AddUnit(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = this.Meta.Pluralize(roleSingularName);

            var roleType = new DynamicUnitRoleType
            (
                objectType,
                roleSingularName,
                rolePluralName,
                roleSingularName,
                true,
                false
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.Meta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicUnitAssociationType(
                this,
                roleType,
                associationSingularName,
                associationPluralName,
                associationSingularName
            );

            this.AddRoleType(roleType);
            objectType.AddAssociationType(roleType.AssociationType);

            this.Meta.ResetDerivations();

            return roleType;
        }

        internal DynamicOneToOneRoleType AddOneToOne(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = this.Meta.Pluralize(roleSingularName);

            var roleType = new DynamicOneToOneRoleType
            (
                objectType,
                roleSingularName,
                rolePluralName,
                roleSingularName,
                true,
                false
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.Meta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicOneToOneAssociationType(
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

            this.Meta.ResetDerivations();

            return roleType;
        }

        internal DynamicManyToOneRoleType AddManyToOne(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = this.Meta.Pluralize(roleSingularName);

            var roleType = new DynamicManyToOneRoleType
            (
                objectType,
                roleSingularName,
                rolePluralName,
                roleSingularName,
                true,
                false
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.Meta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicManyToOneAssociationType
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

            this.Meta.ResetDerivations();

            return roleType;
        }

        internal DynamicOneToManyRoleType AddOneToMany(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = this.Meta.Pluralize(roleSingularName);

            var roleType = new DynamicOneToManyRoleType
            (
                objectType,
                roleSingularName,
                rolePluralName,
                rolePluralName,
                false,
                true
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.Meta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicOneToManyAssociationType(
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

            this.Meta.ResetDerivations();

            return roleType;
        }

        internal DynamicManyToManyRoleType AddManyToMany(DynamicObjectType objectType, string? roleSingularName, string? associationSingularName)
        {
            roleSingularName ??= objectType.Name;
            string rolePluralName = this.Meta.Pluralize(roleSingularName);

            var roleType = new DynamicManyToManyRoleType(
                objectType,
                roleSingularName,
                rolePluralName,
                rolePluralName,
                false,
                true
            );

            string associationPluralName;
            if (associationSingularName != null)
            {
                associationPluralName = this.Meta.Pluralize(associationSingularName);
            }
            else
            {
                associationSingularName = roleType.SingularNameForEmbeddedAssociationType(this);
                associationPluralName = roleType.PluralNameForEmbeddedAssociationType(this);
            }

            roleType.AssociationType = new DynamicManyToManyAssociationType
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

            this.Meta.ResetDerivations();

            return roleType;
        }

        internal void ResetDerivations()
        {
            this.derivedSupertypes = null;
            this.derivedAssociationTypeByName = null;
            this.derivedRoleTypeByName = null;
        }

        private void AddAssociationType(IDynamicAssociationType associationType)
        {
            this.CheckNames(associationType.SingularName, associationType.PluralName);

            this.assignedAssociationTypeByName.Add(associationType.SingularName, associationType);
            this.assignedAssociationTypeByName.Add(associationType.PluralName, associationType);
        }

        private void AddRoleType(IDynamicRoleType roleType)
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

        public bool IsAssignableFrom(DynamicObjectType other)
        {
            return this == other || other.Supertypes.Contains(this);
        }
    }
}
