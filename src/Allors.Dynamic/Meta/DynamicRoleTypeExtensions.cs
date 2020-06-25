namespace Allors.Dynamic
{
    using System;
    using Allors.Dynamic.Meta;

    public static class DynamicRoleTypeExtensions
    {
        public static Func<T, Action<dynamic>> Set<T>(this DynamicRoleType @this)
        {
            Action<dynamic> action(T value)
            {
                return (obj) => ((DynamicObject)obj).Set(@this, value);
            }

            return action;
        }

        public static Func<dynamic, Action<dynamic>> Add(this DynamicRoleType @this)
        {
            var meta = @this.Meta;

            Action<dynamic> action(dynamic role)
            {
                return (obj) => ((DynamicObject)obj).Add(@this, role);
            }

            return action;
        }

        public static Func<dynamic, Action<dynamic>> Remove(this DynamicRoleType @this)
        {
            var meta = @this.Meta;

            Action<dynamic> action(dynamic role)
            {
                return (obj) => ((DynamicObject)obj).Remove(@this, role);
            }

            return action;
        }
    }
}
