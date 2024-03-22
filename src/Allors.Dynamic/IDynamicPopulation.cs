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

        IDynamicObject New(DynamicObjectType @class, params Action<dynamic>[] builders);

        IDynamicObject New(string className, params Action<dynamic>[] builders);

        DynamicChangeSet Snapshot();

        void Derive();

        object GetRole(IDynamicObject obj, IDynamicRoleType roleType);

        void SetRole(IDynamicObject obj, IDynamicRoleType roleType, object value);

        void AddRole(IDynamicObject obj, IDynamicRoleType roleType, IDynamicObject role);

        void RemoveRole(IDynamicObject obj, IDynamicRoleType roleType, IDynamicObject role);

        object GetAssociation(IDynamicObject obj, IDynamicAssociationType associationType);
    }
}
