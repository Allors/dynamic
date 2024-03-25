using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Domain
{
    public sealed class DynamicChangeSet(
        DynamicMeta meta,
        IReadOnlyDictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType,
        IReadOnlyDictionary<IDynamicCompositeAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType)
    {
        private static readonly IReadOnlyDictionary<DynamicObject, object> Empty = new ReadOnlyDictionary<DynamicObject, object>(new Dictionary<DynamicObject, object>());

        public DynamicMeta Meta { get; } = meta;

        public bool HasChanges =>
            roleByAssociationByRoleType.Any(v => v.Value.Count > 0) ||
            associationByRoleByAssociationType.Any(v => v.Value.Count > 0);

        public IReadOnlyDictionary<DynamicObject, object> ChangedRoles(DynamicObjectType objectType, string name)
        {
            var roleType = objectType.RoleTypeByName[name];
            return ChangedRoles(roleType) ?? Empty;
        }

        public IReadOnlyDictionary<DynamicObject, object> ChangedRoles(IDynamicRoleType roleType)
        {
            roleByAssociationByRoleType.TryGetValue(roleType, out var changedRelations);
            return changedRelations ?? Empty;
        }
    }
}
