using System;

namespace Allors.Dynamic
{
    internal static class NullableArraySet
    {
        internal static DynamicObject[] Add(object set, DynamicObject item)
        {
            var typedSet = (DynamicObject[]) set;

            if (typedSet == null)
            {
                return new[] {item};
            }

            Array.Resize(ref typedSet, typedSet.Length + 1);
            typedSet[typedSet.Length - 1] = item;
            return typedSet;
        }

        internal static DynamicObject[] Remove(object set, DynamicObject item)
        {
            var typedSet = (DynamicObject[]) set;

            if (typedSet != null && Array.IndexOf(typedSet, item) > -1)
            {
                if (typedSet.Length == 1)
                {
                    return null;
                }

                var index = Array.IndexOf(typedSet, item);
                typedSet[index] = typedSet[typedSet.Length - 1];
                Array.Resize(ref typedSet, typedSet.Length - 1);
                return typedSet;
            }

            return null;
        }
    }
}