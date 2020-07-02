namespace Allors.Dynamic
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Allors.Dynamic.Meta;

    public interface IDynamicObject 
    {
        void Get(DynamicRoleType roleType, out object result);

        void Get(DynamicAssociationType associationType, out object result);

        void Set(DynamicRoleType roleType, object role);

        void Add(DynamicRoleType roleType, DynamicObject role);

        void Remove(DynamicRoleType roleType, DynamicObject role);

        T GetRole<T>(string name);

        void SetRole<T>(string name, T value);

        T GetAssociation<T>(string name);
    }
}