using Allors.Dynamic.Meta;
using System.Collections.Generic;
using System;

namespace Allors.Dynamic
{
    public interface IDynamicPopulation
    {
        DynamicMeta Meta { get; }

        Dictionary<string, IDynamicDerivation> DerivationById { get; }

        IEnumerable<IDynamicObject> Objects { get; }

        IDynamicObject Create(DynamicObjectType @class, params Action<dynamic>[] builders);

        IDynamicObject Create(string className, params Action<dynamic>[] builders);

        DynamicChangeSet Snapshot();

        void Derive();

        object GetRole(IDynamicObject obj, DynamicUnitRoleType roleType);

        IDynamicObject GetRole(IDynamicObject obj, IDynamicToOneRoleType roleType);

        IReadOnlyList<IDynamicObject> GetRole(IDynamicObject obj, IDynamicToManyRoleType roleType);

        void SetRole(IDynamicObject obj, DynamicUnitRoleType roleType, object value);

        void SetRole(IDynamicObject obj, IDynamicToOneRoleType roleType, IDynamicObject value);

        void SetRole(IDynamicObject obj, IDynamicToManyRoleType roleType, System.Collections.IEnumerable value);

        void AddRole(IDynamicObject obj, IDynamicToManyRoleType roleType, IDynamicObject role);

        void RemoveRole(IDynamicObject obj, IDynamicToManyRoleType roleType, IDynamicObject role);

        IDynamicObject GetAssociation(IDynamicObject obj, IDynamicOneToAssociationType associationType);

        IReadOnlyList<IDynamicObject> GetAssociation(IDynamicObject obj, IDynamicManyToAssociationType associationType);
    }
}
