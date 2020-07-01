using System;

namespace Allors.Dynamic.Meta
{
    public class DynamicObjectType
    {
        internal DynamicObjectType(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }
    }
}