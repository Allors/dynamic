using System.Collections.Concurrent;
using System.Linq;

namespace Allors.Dynamic
{
    public class ChangeSet
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>> changedRoleByAssociationByRelation;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<object, object>> changedAssociationByRoleByRelation;

        public ChangeSet(ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>> changedRoleByAssociationByRelation, ConcurrentDictionary<string, ConcurrentDictionary<object, object>> changedAssociationByRoleByRelation)
        {
            this.changedRoleByAssociationByRelation = changedRoleByAssociationByRelation;
            this.changedAssociationByRoleByRelation = changedAssociationByRoleByRelation;
        }

        public bool HasChanges =>
            this.changedRoleByAssociationByRelation.Any(v => v.Value.Count > 0) ||
            this.changedAssociationByRoleByRelation.Any(v => v.Value.Count > 0);

        public ConcurrentDictionary<AllorsDynamicObject, object> ChangedRoles(string relationName)
        {
            this.changedRoleByAssociationByRelation.TryGetValue(relationName, out var changedRelations);
            return changedRelations;
        }
    }
}
