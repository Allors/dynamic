using Allors.Dynamic.Meta;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Allors.Dynamic
{
    public class DynamicPopulation
    {
        public DynamicMeta Meta { get; }

        public Dictionary<string, IDynamicDerivation> DerivationById { get; }

        private readonly DynamicDatabase database;

        public DynamicPopulation(params Action<DynamicMeta>[] builders)
        {
            this.Meta = new DynamicMeta();

            this.DerivationById = new Dictionary<string, IDynamicDerivation>();

            this.database = new DynamicDatabase(this.Meta);

            foreach (var builder in builders)
            {
                builder?.Invoke(this.Meta);
            }
        }

        public void Derive()
        {
            var changeSet = this.Snapshot();

            while (changeSet.HasChanges)
            {
                foreach (var kvp in this.DerivationById)
                {
                    var derivation = kvp.Value;
                    derivation.Derive(changeSet);
                }

                changeSet = this.Snapshot();
            }
        }

        public DynamicObject Create(params Action<dynamic>[] builders)
        {
            var newObject = new DynamicObject(this);
            this.database.AddObject(newObject);

            foreach (var builder in builders)
            {
                builder(newObject);
            }

            return newObject;
        }

        public DynamicChangeSet Snapshot()
        {
            return this.database.Snapshot();
        }

        public IEnumerable<dynamic> Objects => this.database.Objects;

        internal bool TryGetIndex(DynamicObject obj, GetIndexBinder binder, object[] indexes, out object result) => this.TryGet(obj, indexes[0] as string, out result);

        internal bool TrySetIndex(DynamicObject obj, SetIndexBinder binder, object[] indexes, object value) => this.Set(obj, indexes[0] as string, value);

        internal bool TryGetMember(DynamicObject obj, GetMemberBinder binder, out object result) => this.TryGet(obj, binder.Name, out result);

        internal bool TrySetMember(dynamic obj, SetMemberBinder binder, object value) => this.Set(obj, binder.Name, value);

        internal bool TryInvokeMember(dynamic obj, InvokeMemberBinder binder, object[] args, out object result)
        {
            var name = binder.Name;

            result = null;

            if (name.StartsWith("Add") && this.Meta.LinkedTypeByName.TryGetValue(name.Substring(3), out var roleType))
            {
                this.database.AddLinked(obj, roleType, (DynamicObject)args[0]);
                return true;
            }

            if (name.StartsWith("Remove") && this.Meta.LinkedTypeByName.TryGetValue(name.Substring(6), out roleType))
            {
                // TODO: RemoveAll
                this.database.RemoveLinked(obj, roleType, (DynamicObject)args[0]);
                return true;
            }

            return false;
        }

        private bool TryGet(DynamicObject obj, string name, out object result)
        {
            if (name != null)
            {
                if (this.Meta.LinkedTypeByName.TryGetValue(name, out var linkedType))
                {
                    this.database.GetLinked(obj, linkedType, out result);

                    if (linkedType.IsMany)
                    {
                        result ??= Array.Empty<DynamicObject>();
                    }

                    return true;
                }

                if (this.Meta.LinkerTypeByName.TryGetValue(name, out var linkerType))
                {
                    this.database.GetLinker(obj, linkerType, out result);

                    if (linkerType.IsMany)
                    {
                        result ??= Array.Empty<DynamicObject>();
                    }

                    return true;
                }
            }

            result = null;
            return false;
        }

        private bool Set(dynamic obj, string name, object value)
        {
            if (name != null)
            {
                if (this.Meta.LinkedTypeByName.TryGetValue(name, out var linkedType))
                {
                    this.database.SetLinked(obj, linkedType, value);
                    return true;
                }
            }

            return false;
        }
    }
}
