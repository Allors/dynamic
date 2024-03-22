using Allors.Dynamic.Meta;

namespace Allors.Dynamic
{
    public interface IDynamicObject 
    {
        DynamicPopulation Population { get; }

        DynamicObjectType ObjectType { get; }

        object GetRole(string name);

        void SetRole(string name, object value);

        void AddRole(string name, IDynamicObject value);

        void RemoveRole(string name, IDynamicObject value);

        object GetAssociation(string name);
    }
}