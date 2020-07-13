namespace Allors.Dynamic
{
    public interface IDynamicObject
    {
        object GetRole(string name);

        void SetRole(string name, object value);

        void AddRole(string name, IDynamicObject dynamicObject);

        void RemoveRole(string name, IDynamicObject dynamicObject);

        object GetAssociation(string name);
    }
}
