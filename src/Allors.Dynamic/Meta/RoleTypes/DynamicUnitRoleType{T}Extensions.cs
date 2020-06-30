namespace Allors.Dynamic
{
    using System;
    using Allors.Dynamic.Meta;

    public static class DynamicUnitRoleTypeExtensions
    {
        public static Func<dynamic, T> Get<T>(this DynamicUnitRoleType<T> @this)
        {
            T function(dynamic obj)
            {
                ((DynamicObject)obj).Get(@this, out var result);
                return (T)result;
            }

            return function;
        }

        public static Func<T, Action<dynamic>> Set<T>(this DynamicUnitRoleType<T> @this)
        {
            Action<dynamic> action(T value)
            {
                return (obj) => ((DynamicObject)obj).Set(@this, value);
            }

            return action;
        }
    }
}
