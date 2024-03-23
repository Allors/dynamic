using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Binding
{
    public sealed class DynamicDatabase : Dynamic.DynamicDatabase
    {
        public DynamicDatabase(DynamicMeta meta) : base(meta)
        {
        }

        protected override bool Same(object source, object destination)
        {
            if (source == null && destination == null)
            {
                return true;
            }

            if (source == null || destination == null)
            {
                return false;
            }

            var sourceList = (IReadOnlyList<DynamicObject>)source;
            var destinationList = (IReadOnlyList<DynamicObject>)source;

            if (sourceList.Count != destinationList.Count)
            {
                return false;
            }

            return sourceList.All(v => destinationList.Contains(v));
        }

        protected override IReadOnlyList<IDynamicObject> Add(object set, IDynamicObject item)
        {
            var objects = DynamicObjects.Ensure(set);
            return objects.Add(item);
        }

        protected override IReadOnlyList<IDynamicObject> Remove(object set, IDynamicObject item)
        {
            var objects = DynamicObjects.Ensure(set);
            return objects.Remove(item);
        }
    }
}