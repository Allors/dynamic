using System.Collections.Concurrent;
using System.Globalization;

namespace Allors.Dynamic.Meta
{
    public class DynamicMeta
    {
        private readonly ConcurrentDictionary<string, DynamicAssociationType> associationTypeByName;

        private readonly ConcurrentDictionary<string, DynamicRoleType> roleTypeByName;

        private readonly Inflector.Inflector inflector;

        internal DynamicMeta()
        {
            this.associationTypeByName = new ConcurrentDictionary<string, DynamicAssociationType>();
            this.roleTypeByName = new ConcurrentDictionary<string, DynamicRoleType>();

            this.inflector = new Inflector.Inflector(new CultureInfo("en"));
        }

        public ConcurrentDictionary<string, DynamicAssociationType> AssociationTypeByName => associationTypeByName;

        public ConcurrentDictionary<string, DynamicRoleType> RoleTypeByName => roleTypeByName;

        public DynamicMeta AddRelation(string roleName, string associationName = null)
        {
            var roleType = new DynamicRoleType
            {
                Name = roleName,
            };

            this.RoleTypeByName[roleType.Name] = roleType;

            if (!string.IsNullOrWhiteSpace(associationName))
            {
                var associationType = new DynamicAssociationType(roleType)
                {
                    Name = associationName
                };

                this.AssociationTypeByName[associationType.Name] = associationType;
            }

            return this;
        }
    }
}
