using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Xml.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Indexed
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

        public void AddRole(string name, IDynamicObject value) => Population.AddRole(this, ObjectType.RoleTypeByName[name], value);

        public void RemoveRole(string name, IDynamicObject value) => Population.RemoveRole(this, ObjectType.RoleTypeByName[name], value);

        public object GetAssociation(string name) => Population.GetAssociation(this, ObjectType.AssociationTypeByName[name]);

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
                    return GetAssociation(associationType);
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

        public object this[DynamicRoleType roleType]
        {
            get => GetRole(roleType);
            set => this.Population.SetRole(this, roleType, value);
        }
        
        public object this[DynamicAssociationType associationType] => GetAssociation(associationType);

        private object GetRole(DynamicRoleType roleType)
        {
            var result = Population.GetRole(this, roleType);
            if (result == null && roleType.IsMany)
            {
                result = Array.Empty<DynamicObject>();
            }

            return result;
        }

        private object GetAssociation(DynamicAssociationType associationType)
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