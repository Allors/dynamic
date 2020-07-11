namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections.Generic;

    public class DynamicMeta
    {
        public DynamicMeta(IPluralizer pluralizer)
        {
            this.Pluralizer = pluralizer;
            this.ObjectTypeByType = new Dictionary<Type, DynamicObjectType>();
        }

        public IPluralizer Pluralizer { get; }

        public IDictionary<Type, DynamicObjectType> ObjectTypeByType { get; }

        public DynamicUnitRoleType AddUnit<TAssociation, TRole>(string roleName) => this.GetOrAddObjectType(typeof(TAssociation)).AddUnit(this.GetOrAddObjectType(typeof(TRole)), roleName);

        public DynamicOneToOneRoleType AddOneToOne<TAssociation, TRole>(string roleName) => this.GetOrAddObjectType(typeof(TAssociation)).AddOneToOne(this.GetOrAddObjectType(typeof(TRole)), roleName);

        public DynamicManyToOneRoleType AddManyToOne<TAssociation, TRole>(string roleName) => this.GetOrAddObjectType(typeof(TAssociation)).AddManyToOne(this.GetOrAddObjectType(typeof(TRole)), roleName);

        public DynamicOneToManyRoleType AddOneToMany<TAssociation, TRole>(string roleName) => this.GetOrAddObjectType(typeof(TAssociation)).AddOneToMany(this.GetOrAddObjectType(typeof(TRole)), roleName);

        public DynamicManyToManyRoleType AddManyToMany<TAssociation, TRole>(string roleName) => this.GetOrAddObjectType(typeof(TAssociation)).AddManyToMany(this.GetOrAddObjectType(typeof(TRole)), roleName);

        private DynamicObjectType GetOrAddObjectType(Type type)
        {
            if (!this.ObjectTypeByType.TryGetValue(type, out var objectType))
            {
                objectType = new DynamicObjectType(this, type);
                this.ObjectTypeByType.Add(type, objectType);
            }

            return objectType;
        }

        internal void ResetDerivations()
        {
            foreach(var kvp in this.ObjectTypeByType)
            {
                var objectType = kvp.Value;
                objectType.ResetDerivations();
            }
        }
    }
}