using System.Runtime.CompilerServices;

namespace Allors.Dynamic.Meta
{
    public class DynamicRoleType
    {
        public DynamicAssociationType AssociationType { get; internal set; }

        public string Name { get; set; }

        public bool IsMany { get; set; }

        public bool IsUnit => this.AssociationType == null;

        public bool IsComposite => this.AssociationType != null;

        internal DynamicRoleType()
        {
            this.IsMany = false;
        }
    }
}
