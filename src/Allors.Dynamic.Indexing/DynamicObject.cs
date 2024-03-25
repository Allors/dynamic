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

        public object GetRole(string name) => Population.GetRole(this, ObjectType.RoleTypeByName[name]);

        public void SetRole(string name, object value) => Population.SetRole(this, ObjectType.RoleTypeByName[name], value);

        public void AddRole(string name, IDynamicObject value) => Population.AddRole(this, (IDynamicManyRoleType)ObjectType.RoleTypeByName[name], value);

        public void RemoveRole(string name, IDynamicObject value) => Population.RemoveRole(this, (IDynamicManyRoleType)ObjectType.RoleTypeByName[name], value);

        public object GetAssociation(string name) => Population.GetAssociation(this, (IDynamicCompositeAssociationType)ObjectType.AssociationTypeByName[name]);

        public object this[string name]
        {
            get
            {
                if (ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                {
                    return GetRole(roleType);
                }

                if (ObjectType.AssociationTypeByName.TryGetValue(name, out var associationType))
                {
                    return GetAssociation((IDynamicCompositeAssociationType)associationType);
                }

                return null;
            }
            set
            {
                if (ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                {
                    Population.SetRole(this, roleType, value);
                }
            }
        }

        public object this[IDynamicRoleType roleType]
        {
            get => GetRole(roleType);
            set => this.Population.SetRole(this, roleType, value);
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

        public object this[IDynamicAssociationType associationType] => GetAssociation((IDynamicCompositeAssociationType)associationType);

        public DynamicObject this[DynamicOneToOneAssociationType associationType] => (DynamicObject)GetAssociation(associationType);

        public DynamicObject this[DynamicOneToManyAssociationType associationType] => (DynamicObject)GetAssociation(associationType);

        public IEnumerable<DynamicObject> this[DynamicManyToOneAssociationType associationType] => (IEnumerable<DynamicObject>)GetAssociation(associationType);

        public IEnumerable<DynamicObject> this[DynamicManyToManyAssociationType associationType] => (IEnumerable<DynamicObject>)GetAssociation(associationType);

        private object GetRole(IDynamicRoleType roleType)
        {
            var result = Population.GetRole(this, roleType);
            if (result == null && roleType.IsMany)
            {
                result = Array.Empty<DynamicObject>();
            }

            return result;
        }

        private object GetAssociation(IDynamicCompositeAssociationType associationType)
        {
            var result = Population.GetAssociation(this, associationType);
            if (result == null && associationType.IsMany)
            {
                result = Array.Empty<DynamicObject>();
            }

            return result;
        }
    }
}