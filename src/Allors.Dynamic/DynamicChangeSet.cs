namespace Allors.Dynamic
{
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Dynamic.Meta;

    public class DynamicChangeSet
    {
        private readonly Dictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType;
        private readonly Dictionary<IDynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByRoleType;

        public DynamicChangeSet(DynamicMeta meta, Dictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType, Dictionary<IDynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType)
        {
            this.Meta = meta;
            this.roleByAssociationByRoleType = roleByAssociationByRoleType;
            this.associationByRoleByRoleType = associationByRoleByAssociationType;
        }

        public DynamicMeta Meta { get; }

        public bool HasChanges =>
            this.roleByAssociationByRoleType.Any(v => v.Value.Count > 0) ||
            this.associationByRoleByRoleType.Any(v => v.Value.Count > 0);

        public Dictionary<DynamicObject, object> ChangedRoles<TRole>(string name)
        {
            var objectType = this.Meta.ObjectTypeByType[typeof(TRole)];
            var roleType = objectType.RoleTypeByName[name];
            return this.ChangedRoles(roleType);
        }

        public Dictionary<DynamicObject, object> ChangedRoles(DynamicObjectType objectType, string name)
        {
            var roleType = objectType.RoleTypeByName[name];
            return this.ChangedRoles(roleType);
        }

        public Dictionary<DynamicObject, object> ChangedRoles(IDynamicRoleType roleType)
        {
            this.roleByAssociationByRoleType.TryGetValue(roleType, out var changedRelations);
            return changedRelations;
        }
    }
}
