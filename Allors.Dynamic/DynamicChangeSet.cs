using Allors.Dynamic.Meta;
using System.Collections.Generic;
using System.Linq;

namespace Allors.Dynamic
{
    public class DynamicChangeSet
    {
        public DynamicMeta Meta { get; }

        private readonly Dictionary<DynamicLinkedType, Dictionary<DynamicObject, object>> linkedByLinkerByLinkedType;
        private readonly Dictionary<DynamicLinkerType, Dictionary<DynamicObject, object>> linkerByLinkedByLinkerType;

        public DynamicChangeSet(DynamicMeta meta, Dictionary<DynamicLinkedType, Dictionary<DynamicObject, object>> linkedByLinkerByLinkedType, Dictionary<DynamicLinkerType, Dictionary<DynamicObject, object>> linkerByLinkedByLinkerType)
        {
            this.Meta = meta;
            this.linkedByLinkerByLinkedType = linkedByLinkerByLinkedType;
            this.linkerByLinkedByLinkerType = linkerByLinkedByLinkerType;
        }

        public bool HasChanges =>
            this.linkedByLinkerByLinkedType.Any(v => v.Value.Count > 0) ||
            this.linkerByLinkedByLinkerType.Any(v => v.Value.Count > 0);

        public Dictionary<DynamicObject, object> ChangedLinked(string name)
        {
            var linked = this.Meta.LinkedTypeByName[name];
            return this.ChangedLinked(linked);
        }

        public Dictionary<DynamicObject, object> ChangedLinked(DynamicLinkedType linkedType)
        {
            this.linkedByLinkerByLinkedType.TryGetValue(linkedType, out var changedRelations);
            return changedRelations;
        }
    }
}
