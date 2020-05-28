namespace Allors.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Allors.Dynamic.Meta;

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

            foreach (Action<DynamicMeta> builder in builders)
            {
                builder?.Invoke(this.Meta);
            }
        }

        public void Derive()
        {
            DynamicChangeSet changeSet = this.Snapshot();

            while (changeSet.HasChanges)
            {
                foreach (KeyValuePair<string, IDynamicDerivation> kvp in this.DerivationById)
                {
                    IDynamicDerivation derivation = kvp.Value;
                    derivation.Derive(changeSet);
                }

                changeSet = this.Snapshot();
            }
        }

        public DynamicObject Create(params Action<dynamic>[] builders)
        {
            DynamicObject newObject = new DynamicObject(this);
            this.database.AddObject(newObject);

            foreach (Action<dynamic> builder in builders)
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

        internal bool TryGetIndex(DynamicObject obj, GetIndexBinder binder, object[] indexes, out object result)
        {
            return this.TryGet(obj, indexes[0] as string, out result);
        }

        internal bool TrySetIndex(DynamicObject obj, SetIndexBinder binder, object[] indexes, object value)
        {
            return this.Set(obj, indexes[0] as string, value);
        }

        internal bool TryGetMember(DynamicObject obj, GetMemberBinder binder, out object result)
        {
            return this.TryGet(obj, binder.Name, out result);
        }

        internal bool TrySetMember(dynamic obj, SetMemberBinder binder, object value)
        {
            return this.Set(obj, binder.Name, value);
        }

        internal bool TryInvokeMember(dynamic obj, InvokeMemberBinder binder, object[] args, out object result)
        {
            string name = binder.Name;

            result = null;

            if (name.StartsWith("Add") && this.Meta.RoleTypeByName.TryGetValue(name.Substring(3), out DynamicRoleType roleType))
            {
                this.database.AddRole(obj, roleType, (DynamicObject)args[0]);
                return true;
            }

            if (name.StartsWith("Remove") && this.Meta.RoleTypeByName.TryGetValue(name.Substring(6), out roleType))
            {
                // TODO: RemoveAll
                this.database.RemoveRole(obj, roleType, (DynamicObject)args[0]);
                return true;
            }

            return false;
        }

        private bool TryGet(DynamicObject obj, string name, out object result)
        {
            if (name != null)
            {
                if (this.Meta.RoleTypeByName.TryGetValue(name, out DynamicRoleType roleType))
                {
                    this.database.GetRole(obj, roleType, out result);

                    if (roleType.IsMany)
                    {
                        result ??= Array.Empty<DynamicObject>();
                    }

                    return true;
                }

                if (this.Meta.AssociationTypeByName.TryGetValue(name, out DynamicAssociationType associationType))
                {
                    this.database.GetAssociation(obj, associationType, out result);

                    if (associationType.IsMany)
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
                if (this.Meta.RoleTypeByName.TryGetValue(name, out DynamicRoleType roleType))
                {
                    this.database.SetRole(obj, roleType, value);
                    return true;
                }
            }

            return false;
        }
    }
}
