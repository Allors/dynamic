using Allors.Dynamic.Domain.Indexing;
using Allors.Dynamic.Meta;
using System;
using System.Collections.Generic;

namespace Allors.Dynamic.Domain
{
    public sealed class DynamicObject
    {
        internal DynamicObject(DynamicPopulation population, DynamicObjectType objectType)
        {
            Population = population;
            ObjectType = objectType;
            Database = population.Database;
        }

        public DynamicPopulation Population { get; }

        public DynamicObjectType ObjectType { get; }

        internal DynamicDatabase Database { get; }

        public object this[string name]
        {
            get
            {
                if (ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                {
                    return roleType switch
                    {
                        DynamicUnitRoleType unitRoleType => this.GetRole(unitRoleType),
                        IDynamicToOneRoleType toOneRoleType => this.GetRole(toOneRoleType),
                        IDynamicToManyRoleType toManyRoleType => this.GetRole(toManyRoleType),
                        _ => throw new InvalidOperationException(),
                    };
                }

                if (ObjectType.AssociationTypeByName.TryGetValue(name, out var associationType))
                {
                    return associationType switch
                    {
                        IDynamicOneToAssociationType oneToAssociationType => this.GetAssociation(oneToAssociationType),
                        IDynamicManyToAssociationType oneToAssociationType => this.GetAssociation(oneToAssociationType),
                        _ => throw new InvalidOperationException()
                    };
                }

                throw new ArgumentException("Unknown role or association", nameof(name));
            }
            set
            {
                if (ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                {
                    switch (roleType)
                    {
                        case DynamicUnitRoleType unitRoleType:
                            this.SetRole(unitRoleType, value);
                            return;

                        case IDynamicToOneRoleType toOneRoleType:
                            this.SetRole(toOneRoleType, (DynamicObject)value);
                            return;

                        case IDynamicToManyRoleType toManyRoleType:
                            this.Database.SetRole(this, toManyRoleType, value);
                            return;

                        default:
                            throw new InvalidOperationException();
                    }
                }

                throw new ArgumentException("Unknown role", nameof(name));
            }
        }

        public object this[IDynamicRoleType roleType]
        {
            get => roleType switch
            {
                DynamicUnitRoleType unitRoleType => this.GetRole(unitRoleType),
                IDynamicToOneRoleType toOneRoleType => this.GetRole(toOneRoleType),
                IDynamicToManyRoleType toManyRoleType => this.GetRole(toManyRoleType),
                _ => throw new InvalidOperationException(),
            };
            set
            {
                switch (roleType)
                {
                    case DynamicUnitRoleType unitRoleType:
                        this.SetRole(unitRoleType, value);
                        return;

                    case IDynamicToOneRoleType toOneRoleType:
                        this.SetRole(toOneRoleType, (DynamicObject)value);
                        return;

                    case IDynamicToManyRoleType toManyRoleType:
                        this.Database.SetRole(this, toManyRoleType, value);
                        return;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public object this[DynamicUnitRoleType roleType]
        {
            get => GetRole(roleType);
            set => this.SetRole(roleType, value);
        }

        public DynamicObject this[DynamicOneToOneRoleType roleType]
        {
            get => GetRole(roleType);
            set => this.SetRole(roleType, value);
        }

        public DynamicObject this[DynamicManyToOneRoleType roleType]
        {
            get => GetRole(roleType);
            set => this.SetRole(roleType, value);
        }

        public IEnumerable<DynamicObject> this[DynamicOneToManyRoleType roleType]
        {
            get => GetRole(roleType);
            set => this.Database.SetRole(this, roleType, value);
        }

        public IEnumerable<DynamicObject> this[DynamicManyToManyRoleType roleType]
        {
            get => GetRole(roleType);
            set => this.Database.SetRole(this, roleType, value);
        }

        public object this[IDynamicAssociationType associationType] => associationType switch
        {
            IDynamicOneToAssociationType oneToAssociationType => this.GetAssociation(oneToAssociationType),
            IDynamicManyToAssociationType oneToAssociationType => this.GetAssociation(oneToAssociationType),
            _ => throw new InvalidOperationException()
        };

        public DynamicObject this[DynamicOneToOneAssociationType associationType] => GetAssociation(associationType);

        public DynamicObject this[DynamicOneToManyAssociationType associationType] => GetAssociation(associationType);

        public IEnumerable<DynamicObject> this[DynamicManyToOneAssociationType associationType] => GetAssociation(associationType);

        public IEnumerable<DynamicObject> this[DynamicManyToManyAssociationType associationType] => GetAssociation(associationType);

        public object GetRole(DynamicUnitRoleType roleType) => this.Database.GetRole(this, roleType);

        public DynamicObject GetRole(IDynamicToOneRoleType roleType) => (DynamicObject)this.Database.GetRole(this, roleType);

        public IReadOnlyList<DynamicObject> GetRole(IDynamicToManyRoleType roleType) => (IReadOnlyList<DynamicObject>)this.Database.GetRole(this, roleType) ?? [];

        public void SetRole(DynamicUnitRoleType roleType, object value) => this.Database.SetRole(this, roleType, value);

        public void SetRole(IDynamicToOneRoleType roleType, DynamicObject value) => this.Database.SetRole(this, roleType, value);

        public void SetRole(IDynamicToOneRoleType roleType, IEnumerable<DynamicObject> value) => this.Database.SetRole(this, roleType, value);

        public void AddRole(IDynamicToManyRoleType roleType, DynamicObject role) => this.Database.AddRole(this, roleType, role);

        public void RemoveRole(IDynamicToManyRoleType roleType, DynamicObject role) => this.Database.RemoveRole(this, roleType, role);

        public DynamicObject GetAssociation(IDynamicOneToAssociationType associationType) => (DynamicObject)this.Database.GetAssociation(this, associationType);

        public IReadOnlyList<DynamicObject> GetAssociation(IDynamicManyToAssociationType associationType) => (IReadOnlyList<DynamicObject>)this.Database.GetAssociation(this, associationType) ?? [];
    }
}