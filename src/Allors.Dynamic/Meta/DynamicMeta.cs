namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    public class DynamicMeta
    {
        private readonly IDictionary<Type, DynamicObjectType> objectTypeByType;

        public DynamicMeta()
        {
            this.objectTypeByType = new Dictionary<Type, DynamicObjectType>();
        }

        public IReadOnlyDictionary<Type, DynamicObjectType> ObjectTypeByType => new ReadOnlyDictionary<Type, DynamicObjectType>(this.objectTypeByType);

        public DynamicRoleType AddUnit<TAssociation, TRole>(string roleName, string? associationName = null) => this.GetOrAddObjectType(typeof(TAssociation)).AddUnit(this.GetOrAddObjectType(typeof(TRole)), roleName, associationName);

        public DynamicRoleType AddOneToOne<TAssociation, TRole>(string roleName, string? associationName = null) => this.GetOrAddObjectType(typeof(TAssociation)).AddOneToOne(this.GetOrAddObjectType(typeof(TRole)), roleName, associationName);

        public DynamicRoleType AddManyToOne<TAssociation, TRole>(string roleName, string? associationName = null) => this.GetOrAddObjectType(typeof(TAssociation)).AddManyToOne(this.GetOrAddObjectType(typeof(TRole)), roleName, associationName);

        public DynamicRoleType AddOneToMany<TAssociation, TRole>(string roleName, string? associationName = null) => this.GetOrAddObjectType(typeof(TAssociation)).AddOneToMany(this.GetOrAddObjectType(typeof(TRole)), roleName, associationName);

        public DynamicRoleType AddManyToMany<TAssociation, TRole>(string roleName, string? associationName = null) => this.GetOrAddObjectType(typeof(TAssociation)).AddManyToMany(this.GetOrAddObjectType(typeof(TRole)), roleName, associationName);

        public DynamicObjectType GetOrAddObjectType(Type type)
        {
            if (!this.ObjectTypeByType.TryGetValue(type, out var objectType))
            {
                objectType = new DynamicObjectType(this, type);
                this.objectTypeByType.Add(type, objectType);
            }

            return objectType;
        }

        internal string Pluralize(string singular)
        {
            static bool EndsWith(string word, string ending) => word.EndsWith(ending, StringComparison.InvariantCultureIgnoreCase);

            if (EndsWith(singular, "y") &&
                !EndsWith(singular, "ay") &&
                !EndsWith(singular, "ey") &&
                !EndsWith(singular, "iy") &&
                !EndsWith(singular, "oy") &&
                !EndsWith(singular, "uy"))
            {
                return singular.Substring(0, singular.Length - 1) + "ies";
            }

            if (EndsWith(singular, "us"))
            {
                return singular + "es";
            }

            if (EndsWith(singular, "ss"))
            {
                return singular + "es";
            }

            if (EndsWith(singular, "x") ||
                EndsWith(singular, "ch") ||
                EndsWith(singular, "sh"))
            {
                return singular + "es";
            }

            if (EndsWith(singular, "f") && singular.Length > 1)
            {
                return singular.Substring(0, singular.Length - 1) + "ves";
            }

            if (EndsWith(singular, "fe") && singular.Length > 2)
            {
                return singular.Substring(0, singular.Length - 2) + "ves";
            }

            return singular + "s";
        }

        internal void ResetDerivations()
        {
            foreach ((_, DynamicObjectType? objectType) in this.ObjectTypeByType)
            {
                objectType.ResetDerivations();
            }
        }
    }
}
