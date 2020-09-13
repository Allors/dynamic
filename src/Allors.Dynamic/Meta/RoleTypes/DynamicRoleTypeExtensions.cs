namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class DynamicRoleTypeExtensions
    {
        internal static string SingularNameForAssociation(this IDynamicRoleType @this, DynamicObjectType objectType)
        {
            return $"{objectType.Type.Name}Where{@this.SingularName}";
        }

        internal static string PluralNameForAssociation(this IDynamicRoleType @this, DynamicObjectType objectType)
        {
            return $"{@this.ObjectType.Meta.Pluralizer.Pluralize(objectType.Type.Name)}Where{@this.SingularName}";
        }

        internal static object NormalizeToOne(this IDynamicRoleType @this, object value)
        {
            if (value != null)
            {
                var type = @this.ObjectType.Type;
                if (!type.IsAssignableFrom(value.GetType()))
                {
                    throw new ArgumentException($"{@this.Name} should be a {type.Name} but was a {value.GetType()}");
                }
            }

            return value;
        }

        internal static object NormalizeToMany(this IDynamicRoleType @this, object value)
        {
            if (value == null)
            {
                return value;
            }

            if (value is ICollection collection)
            {
                return @this.NormalizeToMany(collection).ToArray();
            }

            throw new ArgumentException($"{value.GetType()} is not a collection Type");
        }

        private static IEnumerable<dynamic> NormalizeToMany(this IDynamicRoleType @this, ICollection role)
        {
            foreach (var @object in role)
            {
                if (@object != null)
                {
                    var type = @this.ObjectType.Type;

                    if (!type.IsAssignableFrom(@object.GetType()))
                    {
                        throw new ArgumentException($"{@this.Name} should be a {type.Name} but was a {@object.GetType()}");
                    }

                    yield return @object;
                }
            }
        }
    }
}