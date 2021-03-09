namespace Allors.Dynamic
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using Allors.Dynamic.Meta;

    public class DynamicChangeSet
    {
        private static readonly IReadOnlyDictionary<DynamicObject, object> Empty = new ReadOnlyDictionary<DynamicObject, object>(new Dictionary<DynamicObject, object>());

        private readonly IReadOnlyDictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType;
        private readonly IReadOnlyDictionary<IDynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByRoleType;

        public DynamicChangeSet(DynamicMeta meta, IReadOnlyDictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType, IReadOnlyDictionary<IDynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType)
        {
            this.Meta = meta;
            this.roleByAssociationByRoleType = roleByAssociationByRoleType;
            this.associationByRoleByRoleType = associationByRoleByAssociationType;
        }

        public DynamicMeta Meta { get; }

        public bool HasChanges =>
            this.roleByAssociationByRoleType.Any(v => v.Value.Count > 0) ||
            this.associationByRoleByRoleType.Any(v => v.Value.Count > 0);

        public IReadOnlyDictionary<DynamicObject, object> ChangedRoles<TRole>(string name)
        {
            var objectType = this.Meta.ObjectTypeByType[typeof(TRole)];
            var roleType = objectType.RoleTypeByName[name];
            return this.ChangedRoles(roleType) ?? Empty;
        }

        public IReadOnlyDictionary<DynamicObject, object> ChangedRoles(DynamicObjectType objectType, string name)
        {
            var roleType = objectType.RoleTypeByName[name];
            return this.ChangedRoles(roleType) ?? Empty;
        }

        public IReadOnlyDictionary<DynamicObject, object> ChangedRoles(IDynamicRoleType roleType)
        {
            this.roleByAssociationByRoleType.TryGetValue(roleType, out var changedRelations);
            return changedRelations?? Empty;
        }
    }
}
