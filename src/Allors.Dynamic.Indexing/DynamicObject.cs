using System;
using System.Collections.Generic;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Indexing
{
    public sealed class DynamicObject : IDynamicObject
    {
        internal DynamicObject(DynamicPopulation population, DynamicObjectType objectType)
        {
            Population = population;
            ObjectType = objectType;
        }

        IDynamicPopulation IDynamicObject.Population => this.Population;

        public DynamicPopulation Population { get; }


        public DynamicObjectType ObjectType { get; }

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
                            Population.SetRole(this, unitRoleType, value);
                            return;

                        case IDynamicToOneRoleType toOneRoleType:
                            Population.SetRole(this, toOneRoleType, (DynamicObject)value);
                            return;

                        case IDynamicToManyRoleType toManyRoleType:
                            ((IDynamicPopulation)Population).SetRole(this, toManyRoleType, (System.Collections.IEnumerable)value);
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
                        Population.SetRole(this, unitRoleType, value);
                        return;

                    case IDynamicToOneRoleType toOneRoleType:
                        Population.SetRole(this, toOneRoleType, (DynamicObject)value);
                        return;

                    case IDynamicToManyRoleType toManyRoleType:
                        ((IDynamicPopulation)Population).SetRole(this, toManyRoleType, (System.Collections.IEnumerable)value);
                        return;

                    default:
                        throw new InvalidOperationException();
                }
            }
        }

        public object this[DynamicUnitRoleType roleType]
        {
            get => GetRole(roleType);
            set => this.Population.SetRole(this, roleType, value);
        }

        public DynamicObject this[DynamicOneToOneRoleType roleType]
        {
            get => (DynamicObject)GetRole(roleType);
            set => this.Population.SetRole(this, roleType, value);
        }

        public DynamicObject this[DynamicManyToOneRoleType roleType]
        {
            get => (DynamicObject)GetRole(roleType);
            set => this.Population.SetRole(this, roleType, value);
        }

        public IEnumerable<DynamicObject> this[DynamicOneToManyRoleType roleType]
        {
            get => (IEnumerable<DynamicObject>)GetRole(roleType);
            set => this.Population.SetRole(this, roleType, value);
        }

        public IEnumerable<DynamicObject> this[DynamicManyToManyRoleType roleType]
        {
            get => (IEnumerable<DynamicObject>)GetRole(roleType);
            set => this.Population.SetRole(this, roleType, value);
        }

        public object this[IDynamicAssociationType associationType] => associationType switch
        {
            IDynamicOneToAssociationType oneToAssociationType => this.GetAssociation(oneToAssociationType),
            IDynamicManyToAssociationType oneToAssociationType => this.GetAssociation(oneToAssociationType),
            _ => throw new InvalidOperationException()
        };

        public DynamicObject this[DynamicOneToOneAssociationType associationType] => (DynamicObject)GetAssociation(associationType);

        public DynamicObject this[DynamicOneToManyAssociationType associationType] => (DynamicObject)GetAssociation(associationType);

        public IEnumerable<DynamicObject> this[DynamicManyToOneAssociationType associationType] => (IEnumerable<DynamicObject>)GetAssociation(associationType);

        public IEnumerable<DynamicObject> this[DynamicManyToManyAssociationType associationType] => (IEnumerable<DynamicObject>)GetAssociation(associationType);

        object IDynamicObject.GetRole(DynamicUnitRoleType roleType) => Population.GetRole(this, roleType);

        IDynamicObject IDynamicObject.GetRole(IDynamicToOneRoleType roleType) => Population.GetRole(this, roleType);

        IReadOnlyList<IDynamicObject> IDynamicObject.GetRole(IDynamicToManyRoleType roleType) => Population.GetRole(this, roleType);

        void IDynamicObject.SetRole(DynamicUnitRoleType roleType, object value) => Population.SetRole(this, roleType, value);

        void IDynamicObject.SetRole(IDynamicToOneRoleType roleType, IDynamicObject value) => ((IDynamicPopulation)Population).SetRole(this, roleType, value);

        void IDynamicObject.AddRole(IDynamicToManyRoleType roleType, IDynamicObject role) => ((IDynamicPopulation)Population).AddRole(this, roleType, role);

        void IDynamicObject.RemoveRole(IDynamicToManyRoleType roleType, IDynamicObject role) => ((IDynamicPopulation)Population).RemoveRole(this, roleType, role);

        IDynamicObject IDynamicObject.GetAssociation(IDynamicOneToAssociationType associationType) => ((IDynamicPopulation)Population).GetAssociation(this, associationType);

        IReadOnlyList<IDynamicObject> IDynamicObject.GetAssociation(IDynamicManyToAssociationType associationType) => ((IDynamicPopulation)Population).GetAssociation(this, associationType);

        public object GetRole(DynamicUnitRoleType roleType) => Population.GetRole(this, roleType);

        public DynamicObject GetRole(IDynamicToOneRoleType roleType) => Population.GetRole(this, roleType);

        public IReadOnlyList<DynamicObject> GetRole(IDynamicToManyRoleType roleType) => Population.GetRole(this, roleType);

        public void SetRole(DynamicUnitRoleType roleType, object value) => Population.SetRole(this, roleType, value);

        public void SetRole(IDynamicToOneRoleType roleType, DynamicObject value) => Population.SetRole(this, roleType, value);

        public void AddRole(IDynamicToManyRoleType roleType, DynamicObject role) => Population.AddRole(this, roleType, role);

        public void RemoveRole(IDynamicToManyRoleType roleType, DynamicObject role) => Population.RemoveRole(this, roleType, role);

        public DynamicObject GetAssociation(IDynamicOneToAssociationType associationType) => Population.GetAssociation(this, associationType);

        public IReadOnlyList<DynamicObject> GetAssociation(IDynamicManyToAssociationType associationType) => Population.GetAssociation(this, associationType);
    }
}