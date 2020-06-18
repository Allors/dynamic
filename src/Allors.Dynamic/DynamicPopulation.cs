namespace Allors.Dynamic
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Allors.Dynamic.Meta;

    public delegate dynamic New(params Action<dynamic>[] builders);

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

        public dynamic New(params Action<dynamic>[] builders)
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
            return this.TryGet(obj, indexes[0], out result);
        }

        internal bool TrySetIndex(DynamicObject obj, SetIndexBinder binder, object[] indexes, object value)
        {
            return this.TrySet(obj, indexes[0] as string, value);
        }

        internal bool TryGetMember(DynamicObject obj, GetMemberBinder binder, out object result)
        {
            return this.TryGet(obj, binder.Name, out result);
        }

        internal bool TrySetMember(dynamic obj, SetMemberBinder binder, object value)
        {
            return this.TrySet(obj, binder.Name, value);
        }

        internal bool TryInvokeMember(dynamic obj, InvokeMemberBinder binder, object[] args, out object result)
        {
            string name = binder.Name;

            result = null;

            if (name.StartsWith("Add") && this.Meta.RoleTypeByName.TryGetValue(name.Substring(3), out DynamicRoleType roleType))
            {
                this.Add(obj, roleType, (DynamicObject)args[0]);
                return true;
            }

            if (name.StartsWith("Remove") && this.Meta.RoleTypeByName.TryGetValue(name.Substring(6), out roleType))
            {
                // TODO: RemoveAll
                this.Remove(obj, roleType, (DynamicObject)args[0]);
                return true;
            }

            return false;
        }

        internal void Get(DynamicObject obj, DynamicRoleType roleType, out object result)
        {
            this.database.GetRole(obj, roleType, out result);

            if (roleType.IsMany)
            {
                result ??= Array.Empty<DynamicObject>();
            }
        }

        internal void Get(DynamicObject obj, DynamicAssociationType associationType, out object result)
        {
            this.database.GetAssociation(obj, associationType, out result);

            if (associationType.IsMany)
            {
                result ??= Array.Empty<DynamicObject>();
            }
        }

        internal void Set(DynamicObject obj, DynamicRoleType roleType, object role)
        {
            this.database.SetRole(obj, roleType, role);
        }

        internal void Add(DynamicObject obj, DynamicRoleType roleType, DynamicObject role)
        {
            this.database.AddRole(obj, roleType, role);
        }

        internal void Remove(DynamicObject obj, DynamicRoleType roleType, DynamicObject role)
        {
            this.database.RemoveRole(obj, roleType, role);
        }

        private bool TryGet(DynamicObject obj, object nameOrType, out object result)
        {
            switch (nameOrType)
            {
                case string name:
                    {
                        if (this.Meta.RoleTypeByName.TryGetValue(name, out DynamicRoleType roleType))
                        {
                            this.Get(obj, roleType, out result);
                            return true;
                        }

                        if (this.Meta.AssociationTypeByName.TryGetValue(name, out DynamicAssociationType associationType))
                        {
                            this.Get(obj, associationType, out result);
                            return true;
                        }
                    }

                    break;

                case DynamicRoleType roleType:
                    this.Get(obj, roleType, out result);
                    return true;

                case DynamicAssociationType associationType:
                    this.Get(obj, associationType, out result);
                    return true;
            }

            result = null;
            return false;
        }

        private bool TrySet(DynamicObject obj, object nameOrType, object value)
        {
            switch (nameOrType)
            {
                case string name:
                    {
                        if (this.Meta.RoleTypeByName.TryGetValue(name, out DynamicRoleType roleType))
                        {
                            this.Set(obj, roleType, value);
                            return true;
                        }
                    }

                    break;

                case DynamicRoleType roleType:
                    this.Set(obj, roleType, value);
                    return true;
            }

            return false;
        }
    }
}
