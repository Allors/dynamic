using Allors.Dynamic.Meta;
using System.Collections.Concurrent;
using System.Linq;

namespace Allors.Dynamic
{
    public class DynamicChangeSet
    {
        public DynamicMeta Meta { get; }

        private readonly ConcurrentDictionary<DynamicRoleType, ConcurrentDictionary<DynamicObject, object>> changedRoleByAssociationByType;
        private readonly ConcurrentDictionary<DynamicAssociationType, ConcurrentDictionary<object, object>> changedAssociationByRoleByType;

        public DynamicChangeSet(DynamicMeta meta, ConcurrentDictionary<DynamicRoleType, ConcurrentDictionary<DynamicObject, object>> changedRoleByAssociationByType, ConcurrentDictionary<DynamicAssociationType, ConcurrentDictionary<object, object>> changedAssociationByRoleByType)
        {
            this.Meta = meta;
            this.changedRoleByAssociationByType = changedRoleByAssociationByType;
            this.changedAssociationByRoleByType = changedAssociationByRoleByType;
        }

        public bool HasChanges =>
            this.changedRoleByAssociationByType.Any(v => v.Value.Count > 0) ||
            this.changedAssociationByRoleByType.Any(v => v.Value.Count > 0);

        public ConcurrentDictionary<DynamicObject, object> ChangedRoles(string roleName)
        {
            var roleType = this.Meta.RoleTypeByName[roleName];
            return this.ChangedRoles(roleType);
        }

        public ConcurrentDictionary<DynamicObject, object> ChangedRoles(DynamicRoleType roleType)
        {
            this.changedRoleByAssociationByType.TryGetValue(roleType, out var changedRelations);
            return changedRelations;
        }
    }
}
