using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Allors.Dynamic.Binding
{
    public sealed class DynamicObjects : IReadOnlyList<IDynamicObject>, IReadOnlyList<DynamicObject>
    {
        private static readonly DynamicObjects Empty = new(Array.Empty<DynamicObject>());

        private readonly DynamicObject[] objects;

        private DynamicObjects(DynamicObject[] objects)
        {
            this.objects = objects;
        }

        IDynamicObject IReadOnlyList<IDynamicObject>.this[int index] => this.objects[index];

        public DynamicObject this[int index] => this.objects[index];

        IEnumerator<IDynamicObject> IEnumerable<IDynamicObject>.GetEnumerator()
        {
            return ((IEnumerable<IDynamicObject>)this.objects).GetEnumerator();
        }

        IEnumerator<DynamicObject> IEnumerable<DynamicObject>.GetEnumerator()
        {
            return ((IEnumerable<DynamicObject>)this.objects).GetEnumerator();
        }

        public IEnumerator GetEnumerator() => this.objects.GetEnumerator();

        public int Count => this.objects.Length;

        internal static DynamicObjects Ensure(object list)
        {
            if (list == null)
            {
                return Empty;
            }

            return (DynamicObjects)list;
        }

        public DynamicObjects Add(IDynamicObject item)
        {
            if (item == null)
            {
                return this;
            }

            if (this.objects.Contains(item))
            {
                return this;
            }

            var added = new DynamicObject[objects.Length + 1];

            Array.Copy(objects, added, objects.Length);
            added[^1] = (DynamicObject)item;

            return new DynamicObjects(added);
        }

        public DynamicObjects Remove(IDynamicObject item)
        {
            var index = Array.IndexOf(this.objects, (DynamicObject)item);

            if (index < 0)
            {
                return this;
            }

            if (this.objects.Length == 1)
            {
                return Empty;
            }

            var removed = new DynamicObject[objects.Length - 1];

            if (index > 0)
            {
                Array.Copy(objects, 0, removed, 0, index);
            }

            if (index < objects.Length - 1)
            {
                Array.Copy(objects, index + 1, removed, index, objects.Length - index - 1);
            }

            return new DynamicObjects(removed);
        }
    }
}