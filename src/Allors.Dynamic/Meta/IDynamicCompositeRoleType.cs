namespace Allors.Dynamic.Meta
{
    public interface IDynamicCompositeRoleType : IDynamicRoleType
    {
        bool IsOne { get; }

        bool IsMany { get; }
    }
}
