namespace Allors.Dynamic.Meta
{
    using System;

    public interface DynamicRoleType
    {
        DynamicMeta Meta { get; }

        Type Type { get; }

        TypeCode TypeCode { get; }

        DynamicAssociationType AssociationType { get; }

        string Name { get; }

        string SingularName { get; }

        string PluralName { get; }

        bool IsOne { get; }

        bool IsMany { get; }

        bool IsUnit { get; }

        object Normalize(object value);
    }
}