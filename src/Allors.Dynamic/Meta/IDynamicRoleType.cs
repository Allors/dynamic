namespace Allors.Dynamic.Meta
{
    using System;

    public interface IDynamicRoleType
    {
        DynamicObjectType ObjectType { get; }

        IDynamicAssociationType AssociationType { get; }

        string Name { get; }

        string SingularName { get; }

        string PluralName { get; }

        bool IsOne { get; }

        bool IsMany { get; }

        bool IsUnit { get; }

        object Normalize(object value);
    }
}