using Allors.Dynamic.Meta;
using System.Collections.Generic;
using System;

namespace Allors.Dynamic.Domain
{
    public sealed class DynamicPopulation
    {
        public DynamicPopulation(DynamicMeta meta)
        {
            Meta = meta;
            DerivationById = [];
            Database = new DynamicDatabase(Meta);
        }

        public DynamicMeta Meta { get; }

        internal DynamicDatabase Database { get; }

        public Dictionary<string, IDynamicDerivation> DerivationById { get; }

        public IEnumerable<DynamicObject> Objects => Database.Objects;
        
        public DynamicObject Create(DynamicObjectType @class, params Action<DynamicObject>[] builders)
        {
            var @new = new DynamicObject(this, @class);
            this.Database.AddObject(@new);

            foreach (var builder in builders)
            {
                builder(@new);
            }

            return @new;
        }
        
        public DynamicObject Create(string className, params Action<DynamicObject>[] builders)
        {
            var @class = this.Meta.ObjectTypeByName[className];
            return this.Create(@class, builders);
        }

        public DynamicChangeSet Snapshot()
        {
            return this.Database.Snapshot();
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
