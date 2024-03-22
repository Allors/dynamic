using Allors.Dynamic.Meta;

namespace Allors.Dynamic
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
    }
}