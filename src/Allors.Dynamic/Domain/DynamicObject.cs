namespace Allors.Dynamic.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using Allors.Dynamic.Meta;

    public sealed class DynamicObject
    {
        internal DynamicObject(DynamicPopulation population, DynamicObjectType objectType)
        {
            this.Population = population;
            this.ObjectType = objectType;
        }

        public DynamicPopulation Population { get; }

        public DynamicObjectType ObjectType { get; }

        public object? this[string name]
        {
            get
            {
                if (this.ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                {
                    return roleType switch
                    {
                        DynamicUnitRoleType unitRoleType => this.Population.GetRole(this, unitRoleType),
                        IDynamicToOneRoleType toOneRoleType => (DynamicObject?)this.Population.GetRole(this, toOneRoleType),
                        IDynamicToManyRoleType toManyRoleType => (IEnumerable<DynamicObject>?)this.Population.GetRole(this, toManyRoleType) ?? [],
                        _ => throw new InvalidOperationException(),
                    };
                }

                if (this.ObjectType.AssociationTypeByName.TryGetValue(name, out var associationType))
                {
                    return associationType switch
                    {
                        IDynamicOneToAssociationType oneToAssociationType => (DynamicObject?)this.Population.GetAssociation(this, oneToAssociationType),
                        IDynamicManyToAssociationType oneToAssociationType => (IEnumerable<DynamicObject>?)this.Population.GetAssociation(this, oneToAssociationType) ?? [],
                        _ => throw new InvalidOperationException(),
                    };
                }

                throw new ArgumentException("Unknown role or association", nameof(name));
            }

            set
            {
                if (this.ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                {
                    switch (roleType)
                    {
                    case DynamicUnitRoleType unitRoleType:
                        this.Population.SetUnitRole(this, unitRoleType, value);
                        return;

                    case IDynamicToOneRoleType toOneRoleType:
                        DynamicObject? value1 = (DynamicObject?)value;
                        this.Population.SetToOneRole(this, toOneRoleType, value1);
                        return;

                    case IDynamicToManyRoleType toManyRoleType:
                        this.Population.SetToManyRole(this, toManyRoleType, value);
                        return;

                    default:
                        throw new InvalidOperationException();
                    }
                }

                throw new ArgumentException("Unknown role", nameof(name));
            }
        }

        public object? this[IDynamicRoleType roleType]
        {
            get => roleType switch
            {
                DynamicUnitRoleType unitRoleType => this.Population.GetRole(this, unitRoleType),
                IDynamicToOneRoleType toOneRoleType => (DynamicObject?)this.Population.GetRole(this, toOneRoleType),
                IDynamicToManyRoleType toManyRoleType => (IEnumerable<DynamicObject>?)this.Population.GetRole(this, toManyRoleType) ?? [],
                _ => throw new InvalidOperationException(),
            };
            set
            {
                switch (roleType)
                {
                case DynamicUnitRoleType unitRoleType:
                    this.Population.SetUnitRole(this, unitRoleType, value);
                    return;

                case IDynamicToOneRoleType toOneRoleType:
                    DynamicObject? value1 = (DynamicObject?)value;
                    this.Population.SetToOneRole(this, toOneRoleType, value1);
                    return;

                case IDynamicToManyRoleType toManyRoleType:
                    this.Population.SetToManyRole(this, toManyRoleType, value);
                    return;

                default:
                    throw new InvalidOperationException();
                }
            }
        }

        public object? this[DynamicUnitRoleType roleType]
        {
            get => this.Population.GetRole(this, roleType);
            set => this.Population.SetUnitRole(this, roleType, value);
        }

        public DynamicObject? this[DynamicOneToOneRoleType roleType]
        {
            get => (DynamicObject?)this.Population.GetRole(this, roleType);
            set => this.Population.SetToOneRole(this, roleType, value);
        }

        public DynamicObject? this[DynamicManyToOneRoleType roleType]
        {
            get => (DynamicObject?)this.Population.GetRole(this, roleType);
            set => this.Population.SetToOneRole(this, roleType, value);
        }

        public IReadOnlySet<DynamicObject> this[DynamicOneToManyRoleType roleType]
        {
            get => (IReadOnlySet<DynamicObject>?)this.Population.GetRole(this, roleType) ?? ImmutableHashSet<DynamicObject>.Empty;
            set => this.Population.SetToManyRole(this, roleType, value);
        }

        public IReadOnlySet<DynamicObject> this[DynamicManyToManyRoleType roleType]
        {
            get => (IReadOnlySet<DynamicObject>?)this.Population.GetRole(this, roleType) ?? ImmutableHashSet<DynamicObject>.Empty;
            set => this.Population.SetToManyRole(this, roleType, value);
        }

        public object? this[IDynamicAssociationType associationType] => associationType switch
        {
            IDynamicOneToAssociationType oneToAssociationType => (DynamicObject?)this.Population.GetAssociation(this, oneToAssociationType),
            IDynamicManyToAssociationType oneToAssociationType => (IEnumerable<DynamicObject>?)this.Population.GetAssociation(this, oneToAssociationType) ?? [],
            _ => throw new InvalidOperationException()
        };

        public DynamicObject? this[DynamicOneToOneAssociationType associationType] => (DynamicObject?)this.Population.GetAssociation(this, associationType);

        public DynamicObject? this[DynamicOneToManyAssociationType associationType] => (DynamicObject?)this.Population.GetAssociation(this, associationType);

        public IReadOnlySet<DynamicObject> this[DynamicManyToOneAssociationType associationType] => (IReadOnlySet<DynamicObject>?)this.Population.GetAssociation(this, associationType) ?? ImmutableHashSet<DynamicObject>.Empty;

        public IReadOnlySet<DynamicObject> this[DynamicManyToManyAssociationType associationType] => (IReadOnlySet<DynamicObject>?)this.Population.GetAssociation(this, associationType) ?? ImmutableHashSet<DynamicObject>.Empty;

        public void Add(IDynamicToManyRoleType roleType, DynamicObject role) => this.Population.AddRole(this, roleType, role);

        public void Remove(IDynamicToManyRoleType roleType, DynamicObject role) => this.Population.RemoveRole(this, roleType, role);
    }
}
