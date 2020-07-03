namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal static class DynamicRoleTypeExtensions
    {
        internal static string SingularNameForAssociation(this DynamicRoleType @this, Type type)
        {
            return $"{type.Name}Where{@this.SingularName}";
        }

        internal static string PluralNameForAssociation(this DynamicRoleType @this, Type type)
        {
            return $"{@this.Meta.Pluralizer.Pluralize(type.Name)}Where{@this.SingularName}";
        }

        internal static object NormalizeToOne(this DynamicRoleType @this, object value)
        {
            if (value != null)
            {
                if (!@this.Type.IsAssignableFrom(value.GetType()))
                {
                    throw new ArgumentException($"{@this.Name} should be a {@this.Type.Name} but was a {value.GetType()}");
                }
            }

            return value;
        }

        internal static object NormalizeToMany(this DynamicRoleType @this, object value)
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

        private static IEnumerable<dynamic> NormalizeToMany(this DynamicRoleType @this, ICollection role)
        {
            foreach (var @object in role)
            {
                if (@object != null)
                {
                    if (!@this.Type.IsAssignableFrom(@object.GetType()))
                    {
                        throw new ArgumentException($"{@this.Name} should be a {@this.Type.Name} but was a {@object.GetType()}");
                    }

                    yield return @object;
                }
            }
        }
    }
}