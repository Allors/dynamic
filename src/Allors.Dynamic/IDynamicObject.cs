namespace Allors.Dynamic
{
    public interface IDynamicObject
    {
        T GetRole<T>(string name);

        void SetRole<T>(string name, T value);

        T GetAssociation<T>(string name);
    }
}