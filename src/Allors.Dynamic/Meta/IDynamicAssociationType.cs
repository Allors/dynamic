namespace Allors.Dynamic.Meta
{
    using System;

    public interface IDynamicAssociationType
    {
        DynamicObjectType ObjectType { get; }

        IDynamicRoleType RoleType { get; }

        string Name { get; }

        string SingularName { get; }

        string PluralName { get; }

        bool IsOne { get; }

        bool IsMany { get; }
    }
}
