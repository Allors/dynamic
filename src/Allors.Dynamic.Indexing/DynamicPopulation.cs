using System;
using System.Collections.Generic;
using System.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Indexing
{
    public sealed class DynamicPopulation : IDynamicPopulation
    {
        private readonly DynamicDatabase database;

        public DynamicPopulation(DynamicMeta meta)
        {
            Meta = meta;
            DerivationById = [];
            database = new DynamicDatabase(Meta);
        }

        public DynamicMeta Meta { get; }

        public Dictionary<string, IDynamicDerivation> DerivationById { get; }

        IEnumerable<IDynamicObject> IDynamicPopulation.Objects => database.Objects;

        public IEnumerable<DynamicObject> Objects => database.Objects.Cast<DynamicObject>();

        IDynamicObject IDynamicPopulation.Create(DynamicObjectType @class, params Action<dynamic>[] builders)
        {
            return this.Create(@class, builders);
        }

        public DynamicObject Create(DynamicObjectType @class, params Action<DynamicObject>[] builders)
        {
            var @new = new DynamicObject(this, @class);
            database.AddObject(@new);

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

        public DynamicObject Create(string className, params Action<DynamicObject>[] builders)
        {
            var @class = this.Meta.ObjectTypeByName[className];
            return this.Create(@class, builders);
        }


        public DynamicChangeSet Snapshot()
        {
            return database.Snapshot();
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

        public object GetRole(IDynamicObject obj, IDynamicRoleType roleType)
        {
            database.GetRole(obj, roleType, out var result);
            return result;
        }

        public void SetRole(IDynamicObject obj, IDynamicRoleType roleType, object value)
        {
            database.SetRole(obj, roleType, value);
        }

        public void AddRole(IDynamicObject obj, IDynamicCompositeRoleType roleType, IDynamicObject role)
        {
            database.AddRole(obj, roleType, role);
        }

        public void RemoveRole(IDynamicObject obj, IDynamicCompositeRoleType roleType, IDynamicObject role)
        {
            database.RemoveRole(obj, roleType, role);
        }

        public object GetAssociation(IDynamicObject obj, IDynamicCompositeAssociationType associationType)
        {
            database.GetAssociation(obj, associationType, out var result);
            return result;
        }

    
  
      
    }
}