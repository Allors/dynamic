using System;

namespace Allors.Dynamic
{
    internal static class NullableArraySet
    {
        internal static IDynamicObject[] Add(object set, IDynamicObject item)
        {
            var sourceArray = (IDynamicObject[])set;

            if (item == null)
            {
                return sourceArray;
            }

            if (sourceArray == null)
            {
                return new[] { item };
            }

            if (Array.IndexOf(sourceArray, item) >= 0)
            {
                return sourceArray;
            }

            var destinationArray = new IDynamicObject[sourceArray.Length + 1];

            Array.Copy(sourceArray, destinationArray, sourceArray.Length);
            destinationArray[destinationArray.Length - 1] = item;

            return destinationArray;
        }

        internal static IDynamicObject[] Remove(object set, IDynamicObject item)
        {

            var sourceArray = (IDynamicObject[])set;

            if (sourceArray == null)
            {
                return null;
            }

            var index = Array.IndexOf(sourceArray, item);

            if (index < 0)
            {
                return sourceArray;
            }

            if (sourceArray.Length == 1)
            {
                return null;
            }

            var destinationArray = new IDynamicObject[sourceArray.Length - 1];

            if (index > 0)
            {
                Array.Copy(sourceArray, 0, destinationArray, 0, index);
            }

            if (index < sourceArray.Length - 1)
            {
                Array.Copy(sourceArray, index + 1, destinationArray, index, sourceArray.Length - index - 1);
            }

            return destinationArray;
        }

        public static bool Same(object source, object destination)
        {
            if (source == null && destination == null)
            {
                return true;
            }

            if (source == null || destination == null)
            {
                return false;
            }

            var sourceArray = (IDynamicObject[])source;
            var destinationArray = (IDynamicObject[])source;

            if (sourceArray.Length != destinationArray.Length)
            {
                return false;
            }

            return Array.TrueForAll(sourceArray, v => Array.IndexOf(destinationArray, v) >= 0);
        }
    }
}