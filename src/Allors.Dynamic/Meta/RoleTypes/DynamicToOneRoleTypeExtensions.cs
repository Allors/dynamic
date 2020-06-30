namespace Allors.Dynamic
{
    using System;
    using Allors.Dynamic.Meta;

    public static class DynamicToOneRoleTypeExtensions
    {
        public static Func<dynamic, dynamic> Get(this DynamicToOneRoleType @this)
        {
            dynamic function(dynamic obj)
            {
                ((DynamicObject)obj).Get(@this, out var result);
                return (dynamic)result;
            }

            return function;
        }

        public static Func<dynamic, Action<dynamic>> Set(this DynamicToOneRoleType @this)
        {
            Action<dynamic> action(dynamic value)
            {
                return (obj) => ((DynamicObject)obj).Set(@this, value);
            }

            return action;
        }
    }
}
