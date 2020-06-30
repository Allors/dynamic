namespace Allors.Dynamic.Meta
{
    public interface DynamicAssociationType
    {
        DynamicRoleType RoleType { get; }

        string Name { get; }

        string SingularName { get; }

        string PluralName { get; }

        bool IsOne { get; }

        bool IsMany { get; }
    }
}
