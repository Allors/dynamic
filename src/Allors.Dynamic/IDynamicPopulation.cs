namespace Allors.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Allors.Dynamic.Meta;

    public delegate T New<T>(params Action<T>[] builders);

    public interface IDynamicPopulation
    {
        DynamicMeta Meta { get; }

        Dictionary<string, IDynamicDerivation> DerivationById { get; }

        void Derive();

        T New<T>(params Action<T>[] builders)
            where T : DynamicObject;

        dynamic New(Type t, params Action<dynamic>[] builders);

        DynamicChangeSet Snapshot();

        IEnumerable<dynamic> Objects { get; }

        bool TryGetIndex(DynamicObject dynamicObject, GetIndexBinder binder, object[] indexes, out object result);

        bool TrySetIndex(DynamicObject dynamicObject, SetIndexBinder binder, object[] indexes, object value);

        bool TryGetMember(DynamicObject dynamicObject, GetMemberBinder binder, out object result);

        bool TrySetMember(DynamicObject dynamicObject, SetMemberBinder binder, object value);

        bool TryInvokeMember(DynamicObject dynamicObject, InvokeMemberBinder binder, object[] args, out object result);

        T GetRole<T>(DynamicObject dynamicObject, string name);

        void SetRole<T>(DynamicObject dynamicObject, string name, T value);

        T GetAssociation<T>(DynamicObject dynamicObject, string name);
    }
}
