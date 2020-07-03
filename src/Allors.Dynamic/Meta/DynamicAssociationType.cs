namespace Allors.Dynamic.Meta
{
    using System;

    public interface DynamicAssociationType
    {
        DynamicRoleType RoleType { get; }

        Type Type { get; }

        string Name { get; }

        string SingularName { get; }

        string PluralName { get; }

        bool IsOne { get; }

        bool IsMany { get; }
    }
}
