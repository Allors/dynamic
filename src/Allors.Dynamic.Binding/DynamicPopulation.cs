using System;
using System.Collections.Generic;
using System.Data;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Binding
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

        public new IEnumerable<dynamic> Objects => database.Objects;

        IDynamicObject IDynamicPopulation.Create(DynamicObjectType @class, params Action<dynamic>[] builders)
        {
            return this.Create(@class, builders);
        }

        public dynamic Create(DynamicObjectType @class, params Action<dynamic>[] builders)
        {
            var @new = (IDynamicObject)new DynamicObject(this, @class);
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

        public dynamic Create(string className, params Action<dynamic>[] builders)
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

        object IDynamicPopulation.GetRole(IDynamicObject obj, DynamicUnitRoleType roleType) => database.GetRole(obj, roleType);

        IDynamicObject IDynamicPopulation.GetRole(IDynamicObject obj, IDynamicToOneRoleType roleType) => (IDynamicObject)database.GetRole(obj, roleType);

        IReadOnlyList<IDynamicObject> IDynamicPopulation.GetRole(IDynamicObject obj, IDynamicToManyRoleType roleType) => (IReadOnlyList<IDynamicObject>)database.GetRole(obj, roleType) ?? [];

        void IDynamicPopulation.SetRole(IDynamicObject obj, DynamicUnitRoleType roleType, object value) => database.SetRole(obj, roleType, value);

        void IDynamicPopulation.SetRole(IDynamicObject obj, IDynamicToOneRoleType roleType, IDynamicObject value) => database.SetRole(obj, roleType, value);

        void IDynamicPopulation.SetRole(IDynamicObject obj, IDynamicToManyRoleType roleType, System.Collections.IEnumerable value) => database.SetRole(obj, roleType, value);

        void IDynamicPopulation.AddRole(IDynamicObject obj, IDynamicToManyRoleType roleType, IDynamicObject role) => database.AddRole(obj, roleType, role);

        void IDynamicPopulation.RemoveRole(IDynamicObject obj, IDynamicToManyRoleType roleType, IDynamicObject role) => database.RemoveRole(obj, roleType, role);

        IDynamicObject IDynamicPopulation.GetAssociation(IDynamicObject obj, IDynamicOneToAssociationType associationType) => (IDynamicObject)database.GetAssociation(obj, associationType);

        IReadOnlyList<IDynamicObject> IDynamicPopulation.GetAssociation(IDynamicObject obj, IDynamicManyToAssociationType associationType) => (IReadOnlyList<IDynamicObject>)database.GetAssociation(obj, associationType) ?? [];
        
        public object GetRole(DynamicObject obj, DynamicUnitRoleType roleType) => database.GetRole(obj, roleType);

        public DynamicObject GetRole(DynamicObject obj, IDynamicToOneRoleType roleType) => (DynamicObject)database.GetRole(obj, roleType);

        public IReadOnlyList<DynamicObject> GetRole(DynamicObject obj, IDynamicToManyRoleType roleType) => (IReadOnlyList<DynamicObject>)database.GetRole(obj, roleType) ?? [];

        public void SetRole(DynamicObject obj, DynamicUnitRoleType roleType, object value) => database.SetRole(obj, roleType, value);

        public void SetRole(DynamicObject obj, IDynamicToOneRoleType roleType, DynamicObject value) => database.SetRole(obj, roleType, value);

        public void SetRole(DynamicObject obj, IDynamicToManyRoleType roleType, IEnumerable<DynamicObject> value) => database.SetRole(obj, roleType, value);

        public void AddRole(DynamicObject obj, IDynamicToManyRoleType roleType, DynamicObject role) => database.AddRole(obj, roleType, role);

        public void RemoveRole(DynamicObject obj, IDynamicToManyRoleType roleType, DynamicObject role) => database.RemoveRole(obj, roleType, role);

        public DynamicObject GetAssociation(DynamicObject obj, IDynamicOneToAssociationType associationType) => (DynamicObject)database.GetAssociation(obj, associationType);

        public IReadOnlyList<DynamicObject> GetAssociation(DynamicObject obj, IDynamicManyToAssociationType associationType) => (IReadOnlyList<DynamicObject>)database.GetAssociation(obj, associationType) ?? [];
    }
}