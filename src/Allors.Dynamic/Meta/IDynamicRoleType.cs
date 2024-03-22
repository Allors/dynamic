namespace Allors.Dynamic.Meta
{
    public interface IDynamicRoleType
    {
        IDynamicAssociationType AssociationType { get; }

        DynamicObjectType ObjectType { get; }

        string SingularName { get; }

        string PluralName { get; }

        string Name { get; }

        bool IsOne { get; }

        bool IsMany { get; }

        bool IsUnit { get; }

        void Deconstruct(out IDynamicRoleType roleType, out IDynamicAssociationType associationType);

        internal object? Normalize(object? value);
    }
}
