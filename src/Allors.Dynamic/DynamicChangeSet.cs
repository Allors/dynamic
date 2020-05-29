namespace Allors.Dynamic
{
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Dynamic.Meta;

    public class DynamicChangeSet
    {
        private readonly Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType;
        private readonly Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByRoleType;

        public DynamicChangeSet(DynamicMeta meta, Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType, Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType)
        {
            this.Meta = meta;
            this.roleByAssociationByRoleType = roleByAssociationByRoleType;
            this.associationByRoleByRoleType = associationByRoleByAssociationType;
        }

        public DynamicMeta Meta { get; }

        public bool HasChanges =>
            this.roleByAssociationByRoleType.Any(v => v.Value.Count > 0) ||
            this.associationByRoleByRoleType.Any(v => v.Value.Count > 0);

        public Dictionary<DynamicObject, object> ChangedRoles(string name)
        {
            DynamicRoleType roleType = this.Meta.RoleTypeByName[name];
            return this.ChangedRoles(roleType);
        }

        public Dictionary<DynamicObject, object> ChangedRoles(DynamicRoleType roleType)
        {
            this.roleByAssociationByRoleType.TryGetValue(roleType, out Dictionary<DynamicObject, object> changedRelations);
            return changedRelations;
        }
    }
}
