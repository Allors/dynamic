using System;
using System.Collections.Generic;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic
{
   public class DynamicPopulation : IDynamicPopulation
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

        public IEnumerable<IDynamicObject> Objects => database.Objects;

        public IDynamicObject New(DynamicObjectType @class, params Action<dynamic>[] builders)
        {
            var @new = (IDynamicObject)new DynamicObject(this, @class);
            database.AddObject(@new);

            foreach (var builder in builders)
            {
                builder(@new);
            }

            return @new;
        }
        
        public IDynamicObject New(string className, params Action<dynamic>[] builders)
        {
            var @class = this.Meta.ObjectTypeByName[className];
            return this.New(@class, builders);
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

        public object GetRole(IDynamicObject obj, DynamicRoleType roleType)
        {
            database.GetRole(obj, roleType, out var result);
            return result;
        }

        public void SetRole(IDynamicObject obj, DynamicRoleType roleType, object value)
        {
            database.SetRole(obj, roleType, value);
        }

        public void AddRole(IDynamicObject obj, DynamicRoleType roleType, IDynamicObject role)
        {
            database.AddRole(obj, roleType, role);
        }

        public void RemoveRole(IDynamicObject obj, DynamicRoleType roleType, IDynamicObject role)
        {
            database.RemoveRole(obj, roleType, role);
        }

        public object GetAssociation(IDynamicObject obj, DynamicAssociationType associationType)
        {
            database.GetAssociation(obj, associationType, out var result);
            return result;
        }
    }
}
