using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic
{
    public class DynamicObject : IDynamicObject
    {
        internal DynamicObject(DynamicPopulation population, DynamicObjectType objectType)
        {
            Population = population;
            ObjectType = objectType;
        }

        public DynamicPopulation Population { get; }

        public DynamicObjectType ObjectType { get; }

        public object GetRole(string name) => Population.GetRole(this, ObjectType.RoleTypeByName[name]);

        public void SetRole(string name, object value) => Population.SetRole(this, ObjectType.RoleTypeByName[name], value);

        public void AddRole(string name, IDynamicObject value) => Population.AddRole(this, ObjectType.RoleTypeByName[name], value);

        public void RemoveRole(string name, IDynamicObject value) => Population.RemoveRole(this, ObjectType.RoleTypeByName[name], value);

        public object GetAssociation(string name) => Population.GetAssociation(this, ObjectType.AssociationTypeByName[name]);
    }
}