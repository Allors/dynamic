namespace Allors.Dynamic
{
    using System;
    using System.Collections.Generic;
    using Allors.Dynamic.Meta;

    public delegate T New<T>(params Action<T>[] builders);

    public interface IDynamicPopulation
    {
        DynamicMeta Meta { get; }

        IEnumerable<dynamic> Objects { get; }

        Dictionary<string, IDynamicDerivation> DerivationById { get; }

        T New<T>(params Action<T>[] builders)
            where T : DynamicObject;

        dynamic New(Type t, params Action<dynamic>[] builders);

        object GetRole(DynamicObject dynamicObject, DynamicRoleType roleType);

        void SetRole(DynamicObject dynamicObject, DynamicRoleType roleType, object value);

        void AddRole(DynamicObject obj, DynamicRoleType roleType, IDynamicObject dynamicObject);

        void RemoveRole(DynamicObject obj, DynamicRoleType roleType, IDynamicObject dynamicObject);

        object GetAssociation(DynamicObject dynamicObject, DynamicAssociationType associationType);

        DynamicChangeSet Snapshot();

        void Derive();
    }
}
