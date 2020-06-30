namespace Allors.Dynamic.Meta
{
    public interface DynamicRoleType
    {
        DynamicMeta Meta { get; }

        DynamicAssociationType AssociationType { get; }

        string Name { get; }

        string SingularName { get; }

        string PluralName { get; }

        bool IsOne { get; }

        bool IsMany { get; }

        bool IsUnit { get; }
    }
}