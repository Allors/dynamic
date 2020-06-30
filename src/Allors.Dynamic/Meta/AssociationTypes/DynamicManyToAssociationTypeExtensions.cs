using System;

namespace Allors.Dynamic.Meta
{
    public static class DynamicManyToAssociationTypeExtensions
    {
        public static Func<dynamic, dynamic[]> Get(this DynamicManyToAssociationType @this)
        {
            dynamic[] function(dynamic obj)
            {
                ((DynamicObject)obj).Get(@this, out var result);
                return (dynamic[])result;
            }

            return function;
        }
    }
}
