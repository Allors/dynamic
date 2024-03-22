namespace Allors.Dynamic.Meta
{
    public interface IDynamicAssociationType
    {
        DynamicObjectType ObjectType { get; }

        IDynamicRoleType RoleType { get; }

        string SingularName { get; }

        string PluralName { get; }

        string Name { get; }

        bool IsOne { get; }

        bool IsMany { get; }
    }
}
