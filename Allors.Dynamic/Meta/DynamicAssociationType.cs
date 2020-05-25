using System.Threading;

namespace Allors.Dynamic.Meta
{
    public class DynamicAssociationType
    {
        public DynamicRoleType RoleType { get; }

        public string Name { get; internal set; }

        public bool IsOne => !this.IsMany;

        public bool IsMany { get; internal set; }

        public DynamicAssociationType(DynamicRoleType roleType)
        {
            roleType.AssociationType = this;
            RoleType = roleType;
        }
    }
}
