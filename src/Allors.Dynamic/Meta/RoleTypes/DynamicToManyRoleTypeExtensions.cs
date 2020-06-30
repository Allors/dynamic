namespace Allors.Dynamic
{
    using System;
    using Allors.Dynamic.Meta;

    public static class DynamicToManyRoleTypeExtensions
    {
        public static Func<dynamic, dynamic[]> Get(this DynamicToManyRoleType @this)
        {
            dynamic[] function(dynamic obj)
            {
                ((DynamicObject)obj).Get(@this, out var result);
                return (dynamic[])result;
            }

            return function;
        }

        public static Func<dynamic[], Action<dynamic>> Set(this DynamicToManyRoleType @this)
        {
            Action<dynamic> action(dynamic[] value)
            {
                return (obj) => ((DynamicObject)obj).Set(@this, value);
            }

            return action;
        }

        public static Func<dynamic, Action<dynamic>> Add(this DynamicToManyRoleType @this)
        {
            var meta = @this.Meta;

            Action<dynamic> action(dynamic role)
            {
                return (obj) => ((DynamicObject)obj).Add(@this, role);
            }

            return action;
        }

        public static Func<dynamic, Action<dynamic>> Remove(this DynamicToManyRoleType @this)
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
