using System;
using System.Collections.Generic;
using System.Data;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Binding
{
    public sealed class DynamicPopulation : IDynamicPopulation
    {
        public DynamicPopulation(DynamicMeta meta)
        {
            Meta = meta;
            DerivationById = [];
            Database = new DynamicDatabase(Meta);
        }

        public DynamicMeta Meta { get; }

        public Dictionary<string, IDynamicDerivation> DerivationById { get; }

        internal DynamicDatabase Database { get; }

        IEnumerable<IDynamicObject> IDynamicPopulation.Objects => Database.Objects;

        public new IEnumerable<dynamic> Objects => Database.Objects;

        IDynamicObject IDynamicPopulation.Create(DynamicObjectType @class, params Action<dynamic>[] builders)
        {
            return this.Create(@class, builders);
        }

        public dynamic Create(DynamicObjectType @class, params Action<dynamic>[] builders)
        {
            var @new = (IDynamicObject)new DynamicObject(this, @class);
            Database.AddObject(@new);

            foreach (var builder in builders)
            {
                builder(@new);
            }

            return @new;
        }

        IDynamicObject IDynamicPopulation.Create(string className, params Action<dynamic>[] builders)
        {
            return this.Create(className, builders);
        }

        public dynamic Create(string className, params Action<dynamic>[] builders)
        {
            var @class = this.Meta.ObjectTypeByName[className];
            return this.Create(@class, builders);
        }

        public DynamicChangeSet Snapshot()
        {
            return Database.Snapshot();
        }

        public void Derive()
        {
            var changeSet = Snapshot();

            while (changeSet.HasChanges)
            {
                foreach (var kvp in DerivationById)
                {
                    var derivation = kvp.Value;
                    derivation.Derive(changeSet);
                }

                changeSet = Snapshot();
            }
        }
    }
}