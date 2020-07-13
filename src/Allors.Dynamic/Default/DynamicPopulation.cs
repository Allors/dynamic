namespace Allors.Dynamic.Default
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Allors.Dynamic.Meta;
    using DynamicObject = Allors.Dynamic.DynamicObject;

    public class DynamicPopulation : IDynamicPopulation
    {
        private readonly DynamicDatabase database;

        public DynamicPopulation(DynamicMeta meta, params Action<DynamicMeta>[] builders)
        {
            this.Meta = meta;
            this.DerivationById = new Dictionary<string, IDynamicDerivation>();
            this.database = new DynamicDatabase(this.Meta);

            foreach (Action<DynamicMeta> builder in builders)
            {
                builder?.Invoke(this.Meta);
            }
        }

        public DynamicMeta Meta { get; }

        public Dictionary<string, IDynamicDerivation> DerivationById { get; }

        public dynamic New(Type t, params Action<dynamic>[] builders)
        {
            dynamic @new = Activator.CreateInstance(t, new object[] { this, this.Meta.GetOrAddObjectType(t) });
            this.database.AddObject(@new);

            foreach (var builder in builders)
            {
                builder(@new);
            }

            return @new;
        }

        public T New<T>(params Action<T>[] builders)
              where T : DynamicObject
        {
            T @new = (T)Activator.CreateInstance(typeof(T), new object[] { this, this.Meta.GetOrAddObjectType(typeof(T)) });
            this.database.AddObject(@new);

            foreach (var builder in builders)
            {
                builder(@new);
            }

            return @new;
        }

        public DynamicChangeSet Snapshot()
        {
            return this.database.Snapshot();
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

        public IEnumerable<dynamic> Objects => this.database.Objects;

        public object GetRole(DynamicObject obj, IDynamicRoleType roleType)
        {
            this.database.GetRole(obj, roleType, out var result);
            return result;
        }

        public void SetRole(DynamicObject obj, IDynamicRoleType roleType, object value)
        {
            this.database.SetRole(obj, roleType, value);
        }

        public void AddRole(DynamicObject obj, IDynamicRoleType roleType, IDynamicObject role)
        {
            this.database.AddRole(obj, roleType, (DynamicObject)role);
        }

        public void RemoveRole(DynamicObject obj, IDynamicRoleType roleType, IDynamicObject role)
        {
            this.database.RemoveRole(obj, roleType, (DynamicObject)role);
        }

        public object GetAssociation(DynamicObject obj, IDynamicAssociationType associationType)
        {
            this.database.GetAssociation(obj, associationType, out var result);
            return result;
        }
    }
}
