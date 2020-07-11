namespace Allors.Dynamic.Meta
{
    using System;

    public interface IDynamicObjectType
    {
        DynamicMeta Meta { get; }

        Type Type { get; }

        TypeCode TypeCode { get; }
    }
}