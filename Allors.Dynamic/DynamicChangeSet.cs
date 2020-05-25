using Allors.Dynamic.Meta;
using System.Collections.Generic;
using System.Linq;

namespace Allors.Dynamic
{
    public class DynamicChangeSet
    {
        public DynamicMeta Meta { get; }

        private readonly Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> changedRoleByAssociationByType;
        private readonly Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> changedAssociationByRoleByType;

        public DynamicChangeSet(DynamicMeta meta, Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> changedRoleByAssociationByType, Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> changedAssociationByRoleByType)
        {
            this.Meta = meta;
            this.changedRoleByAssociationByType = changedRoleByAssociationByType;
            this.changedAssociationByRoleByType = changedAssociationByRoleByType;
        }

        public bool HasChanges =>
            this.changedRoleByAssociationByType.Any(v => v.Value.Count > 0) ||
            this.changedAssociationByRoleByType.Any(v => v.Value.Count > 0);

        public Dictionary<DynamicObject, object> ChangedRoles(string roleName)
        {
            var roleType = this.Meta.RoleTypeByName[roleName];
            return this.ChangedRoles(roleType);
        }

        public Dictionary<DynamicObject, object> ChangedRoles(DynamicRoleType roleType)
        {
            this.changedRoleByAssociationByType.TryGetValue(roleType, out var changedRelations);
            return changedRelations;
        }
    }
}
