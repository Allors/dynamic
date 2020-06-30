namespace Allors.Dynamic.Meta
{
    using System;

    public static class DynamicOneToAssociationTypeExtensions
    {
        public static Func<dynamic, dynamic> Get(this DynamicOneToAssociationType @this)
        {
            dynamic function(dynamic obj)
            {
                ((DynamicObject)obj).Get(@this, out var result);
                return (dynamic)result;
            }

            return function;
        }
    }
}
