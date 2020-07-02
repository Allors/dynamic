namespace Allors.Dynamic.Meta
{
    using System;

    public class DynamicObjectType
    {
        internal DynamicObjectType(Type type)
        {
            this.Type = type;
        }

        public Type Type { get; }
    }
}