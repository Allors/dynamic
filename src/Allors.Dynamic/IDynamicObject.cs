using Allors.Dynamic.Meta;
using System.Collections.Generic;

namespace Allors.Dynamic
{
    public interface IDynamicObject 
    {
        IDynamicPopulation Population { get; }

        DynamicObjectType ObjectType { get; }

        object GetRole(DynamicUnitRoleType roleType);

        IDynamicObject GetRole(IDynamicToOneRoleType roleType);

        IReadOnlyList<IDynamicObject> GetRole(IDynamicToManyRoleType roleType);

        void SetRole(DynamicUnitRoleType roleType, object value);

        void SetRole(IDynamicToOneRoleType roleType, IDynamicObject value);

        void AddRole(IDynamicToManyRoleType roleType, IDynamicObject role);

        void RemoveRole(IDynamicToManyRoleType roleType, IDynamicObject role);

        IDynamicObject GetAssociation(IDynamicOneToAssociationType associationType);

        IReadOnlyList<IDynamicObject> GetAssociation(IDynamicManyToAssociationType associationType);
    }
}