using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Allors.Dynamic.Meta
{
    public sealed class DynamicMeta
    {
        private readonly Dictionary<string, DynamicObjectType> objectTypeByName;

        public DynamicMeta()
        {
            objectTypeByName = [];

            this.ObjectTypeByName = new ReadOnlyDictionary<string, DynamicObjectType>(this.objectTypeByName);
        }

        public IReadOnlyDictionary<string, DynamicObjectType> ObjectTypeByName { get; }

        public DynamicRoleType AddUnit<TRole>(DynamicObjectType associationObjectType, string roleName, string? associationName = null) => associationObjectType.AddUnit(Unit(typeof(TRole)), roleName, associationName);

        public DynamicRoleType AddOneToOne(DynamicObjectType associationObjectType, DynamicObjectType roleObjectType, string roleName, string? associationName = null) => associationObjectType.AddOneToOne(roleObjectType, roleName, associationName);

        public DynamicRoleType AddManyToOne(DynamicObjectType associationObjectType, DynamicObjectType roleObjectType, string roleName, string? associationName = null) => associationObjectType.AddManyToOne(roleObjectType, roleName, associationName);

        public DynamicRoleType AddOneToMany(DynamicObjectType associationObjectType, DynamicObjectType roleObjectType, string roleName, string? associationName = null) => associationObjectType.AddOneToMany(roleObjectType, roleName, associationName);

        public DynamicRoleType AddManyToMany(DynamicObjectType associationObjectType, DynamicObjectType roleObjectType, string roleName, string? associationName = null) => associationObjectType.AddManyToMany(roleObjectType, roleName, associationName);

        public DynamicObjectType AddInterface(string name, params DynamicObjectType[] supertypes)
        {
            var objectType = new DynamicObjectType(this, DynamicObjectTypeKind.Interface, name, supertypes);
            objectTypeByName.Add(objectType.Name, objectType);
            return objectType;
        }

        public DynamicObjectType AddClass(string name, params DynamicObjectType[] supertypes)
        {
            var objectType = new DynamicObjectType(this, DynamicObjectTypeKind.Class, name, supertypes);
            objectTypeByName.Add(objectType.Name, objectType);
            return objectType;
        }

        private DynamicObjectType Unit(Type type)
        {
            if (!ObjectTypeByName.TryGetValue(type.Name, out var objectType))
            {
                objectType = new DynamicObjectType(this, type);
                objectTypeByName.Add(objectType.Name, objectType);
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
            foreach ((_, DynamicObjectType? objectType) in ObjectTypeByName)
            {
                objectType.ResetDerivations();
            }
        }
    }
}
