using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic
{
    public sealed class DynamicChangeSet
    {
        private static readonly IReadOnlyDictionary<IDynamicObject, object> Empty = new ReadOnlyDictionary<IDynamicObject, object>(new Dictionary<IDynamicObject, object>());

        private readonly IReadOnlyDictionary<DynamicRoleType, Dictionary<IDynamicObject, object>> roleByAssociationByRoleType;
        private readonly IReadOnlyDictionary<DynamicAssociationType, Dictionary<IDynamicObject, object>> associationByRoleByRoleType;

        public DynamicChangeSet(DynamicMeta meta, IReadOnlyDictionary<DynamicRoleType, Dictionary<IDynamicObject, object>> roleByAssociationByRoleType, IReadOnlyDictionary<DynamicAssociationType, Dictionary<IDynamicObject, object>> associationByRoleByAssociationType)
        {
            Meta = meta;
            this.roleByAssociationByRoleType = roleByAssociationByRoleType;
            associationByRoleByRoleType = associationByRoleByAssociationType;
        }

        public DynamicMeta Meta { get; }

        public bool HasChanges =>
            roleByAssociationByRoleType.Any(v => v.Value.Count > 0) ||
            associationByRoleByRoleType.Any(v => v.Value.Count > 0);

        public IReadOnlyDictionary<IDynamicObject, object> ChangedRoles(DynamicObjectType objectType, string name)
        {
            var roleType = objectType.RoleTypeByName[name];
            return ChangedRoles(roleType) ?? Empty;
        }

        public IReadOnlyDictionary<IDynamicObject, object> ChangedRoles(DynamicRoleType roleType)
        {
            roleByAssociationByRoleType.TryGetValue(roleType, out var changedRelations);
            return changedRelations?? Empty;
        }
    }
}
