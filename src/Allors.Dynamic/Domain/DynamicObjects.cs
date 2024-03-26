namespace Allors.Dynamic.Domain.Indexing
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public sealed class DynamicObjects : IReadOnlyList<DynamicObject>
    {
        private static readonly DynamicObjects Empty = new(Array.Empty<DynamicObject>());

        private readonly DynamicObject[] objects;

        private DynamicObjects(DynamicObject[] objects)
        {
            this.objects = objects;
        }

        public DynamicObject this[int index] => this.objects[index];

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

        public DynamicObjects Add(DynamicObject item)
        {
            if (item == null)
            {
                return this;
            }

            if (this.objects.Contains(item))
            {
                return this;
            }

            var added = new DynamicObject[this.objects.Length + 1];

            Array.Copy(this.objects, added, this.objects.Length);
            added[^1] = (DynamicObject)item;

            return new DynamicObjects(added);
        }

        public DynamicObjects Remove(DynamicObject item)
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

            var removed = new DynamicObject[this.objects.Length - 1];

            if (index > 0)
            {
                Array.Copy(this.objects, 0, removed, 0, index);
            }

            if (index < this.objects.Length - 1)
            {
                Array.Copy(this.objects, index + 1, removed, index, this.objects.Length - index - 1);
            }

            return new DynamicObjects(removed);
        }
    }
}