namespace Allors.Dynamic.Meta
{
    public interface IDynamicCompositeAssociationType : IDynamicAssociationType
    {
        bool IsOne { get; }

        bool IsMany { get; }
    }
}
