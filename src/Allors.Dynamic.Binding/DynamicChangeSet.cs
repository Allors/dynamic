using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic
{
    public class DynamicChangeSet
    {
        private static readonly IReadOnlyDictionary<DynamicObject, object> Empty = new ReadOnlyDictionary<DynamicObject, object>(new Dictionary<DynamicObject, object>());

        private readonly IReadOnlyDictionary<DynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType;
        private readonly IReadOnlyDictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByRoleType;

        public DynamicChangeSet(DynamicMeta meta, IReadOnlyDictionary<DynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType, IReadOnlyDictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType)
        {
            Meta = meta;
            this.roleByAssociationByRoleType = roleByAssociationByRoleType;
            associationByRoleByRoleType = associationByRoleByAssociationType;
        }

        public DynamicMeta Meta { get; }

        public bool HasChanges =>
            roleByAssociationByRoleType.Any(v => v.Value.Count > 0) ||
            associationByRoleByRoleType.Any(v => v.Value.Count > 0);

        public IReadOnlyDictionary<DynamicObject, object> ChangedRoles(DynamicObjectType objectType, string name)
        {
            var roleType = objectType.RoleTypeByName[name];
            return ChangedRoles(roleType) ?? Empty;
        }

        public IReadOnlyDictionary<DynamicObject, object> ChangedRoles(DynamicRoleType roleType)
        {
            roleByAssociationByRoleType.TryGetValue(roleType, out var changedRelations);
            return changedRelations?? Empty;
        }
    }
}
