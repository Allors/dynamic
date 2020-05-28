using Allors.Dynamic.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Allors.Dynamic
{
    internal class DynamicDatabase
    {
        private readonly DynamicMeta meta;

        private readonly Dictionary<DynamicLinkedType, Dictionary<DynamicObject, object>> linkedByLinkerByLinkedType;
        private readonly Dictionary<DynamicLinkerType, Dictionary<DynamicObject, object>> linkingByLinkerByLinkingType;

        private Dictionary<DynamicLinkedType, Dictionary<DynamicObject, object>> changedLinkedByLinkerByLinkedType;
        private Dictionary<DynamicLinkerType, Dictionary<DynamicObject, object>> changedLinkerByLinkedByLinkerType;

        private DynamicObject[] objects;

        internal DynamicDatabase(DynamicMeta meta)
        {
            this.meta = meta;

            this.linkedByLinkerByLinkedType = new Dictionary<DynamicLinkedType, Dictionary<DynamicObject, object>>();
            this.linkingByLinkerByLinkingType = new Dictionary<DynamicLinkerType, Dictionary<DynamicObject, object>>();

            this.changedLinkedByLinkerByLinkedType =
                new Dictionary<DynamicLinkedType, Dictionary<DynamicObject, object>>();
            this.changedLinkerByLinkedByLinkerType =
                new Dictionary<DynamicLinkerType, Dictionary<DynamicObject, object>>();
        }

        internal DynamicObject[] Objects => this.objects;

        internal void AddObject(DynamicObject newObject)
        {
            this.objects = NullableArraySet.Add(this.objects, newObject);
        }

        internal void GetLinked(DynamicObject obj, DynamicLinkedType linkedType, out object result)
        {
            if (this.changedLinkedByLinkerByLinkedType.TryGetValue(linkedType, out var changeLinkedByLinking) &&
                changeLinkedByLinking.TryGetValue(obj, out result))
            {
                return;
            }

            var linkedByLinking = GetlinkedByLinker(linkedType);
            linkedByLinking.TryGetValue(obj, out result);
        }

        internal DynamicChangeSet Snapshot()
        {
            foreach (var linkedType in this.changedLinkedByLinkerByLinkedType.Keys.ToArray())
            {
                var changedLinkedByLinker = this.changedLinkedByLinkerByLinkedType[linkedType];

                var linkedByLinker = GetlinkedByLinker(linkedType);
                foreach (var linker in changedLinkedByLinker.Keys.ToArray())
                {
                    var linked = changedLinkedByLinker[linker];
                    linkedByLinker.TryGetValue(linker, out var originalLinked);

                    var areEqual = ReferenceEquals(originalLinked, linked) ||
                                   (linkedType.IsOne && Equals(originalLinked, linked)) ||
                                   (linkedType.IsMany &&
                                    ((IStructuralEquatable) originalLinked)?.Equals((IStructuralEquatable) linked) ==
                                    true);

                    if (areEqual)
                    {
                        changedLinkedByLinker.Remove(linker);
                        continue;
                    }

                    linkedByLinker[linker] = linked;
                }

                if (linkedByLinker.Count == 0)
                {
                    this.changedLinkedByLinkerByLinkedType.Remove(linkedType);
                }
            }

            foreach (var linkerType in this.changedLinkerByLinkedByLinkerType.Keys.ToArray())
            {
                var changedLinkerByLinked = this.changedLinkerByLinkedByLinkerType[linkerType];

                var linkerByLinked = GetLinkerByLinked(linkerType);
                foreach (var role in changedLinkerByLinked.Keys.ToArray())
                {
                    var changedLinker = changedLinkerByLinked[role];
                    linkerByLinked.TryGetValue(role, out var originalLinker);

                    var areEqual = ReferenceEquals(originalLinker, changedLinker) ||
                                   (linkerType.IsOne && Equals(originalLinker, changedLinker)) ||
                                   (linkerType.IsMany &&
                                    ((IStructuralEquatable) originalLinker)?.Equals(
                                        (IStructuralEquatable) changedLinker) == true);

                    if (areEqual)
                    {
                        changedLinkerByLinked.Remove(role);
                        continue;
                    }

                    linkerByLinked[role] = changedLinker;
                }

                if (linkerByLinked.Count == 0)
                {
                    this.changedLinkerByLinkedByLinkerType.Remove(linkerType);
                }
            }

            var snapshot = new DynamicChangeSet(this.meta, this.changedLinkedByLinkerByLinkedType,
                this.changedLinkerByLinkedByLinkerType);

            this.changedLinkedByLinkerByLinkedType =
                new Dictionary<DynamicLinkedType, Dictionary<DynamicObject, object>>();
            this.changedLinkerByLinkedByLinkerType =
                new Dictionary<DynamicLinkerType, Dictionary<DynamicObject, object>>();

            return snapshot;
        }

        internal void SetLinked(dynamic linker, DynamicLinkedType linkedType, object linked)
        {
            if (linkedType.IsData)
            {
                // Linked
                var changedRoleByAssociation = GetChangedLinkedByLinker(linkedType);
                changedRoleByAssociation[linker] = linked;
            }
            else
            {
                var linkerType = linkedType.LinkerType;

                this.GetLinked(linker, linkedType, out object previousLinked);

                if (linkedType.IsOne)
                {
                    var linkedObject = (DynamicObject) linked;
                    this.GetLinker(linkedObject, linkerType, out object previousLinker);

                    // Linked
                    var changedLinkedByLinker = GetChangedLinkedByLinker(linkedType);
                    changedLinkedByLinker[linker] = linkedObject;

                    // Linker
                    var changedLinkerByLinked = GetChangedLinkerByLinked(linkerType);
                    if (linkerType.IsOne)
                    {
                        // One to One
                        var previousLinkerObject = (DynamicObject) previousLinker;
                        if (previousLinkerObject != null)
                        {
                            changedLinkedByLinker[previousLinkerObject] = null;
                        }

                        if (previousLinked != null)
                        {
                            var previousLinkedObject = (DynamicObject) previousLinked;
                            changedLinkerByLinked[previousLinkedObject] = null;
                        }

                        changedLinkerByLinked[linkedObject] = linker;
                    }
                    else
                    {
                        changedLinkerByLinked[linkedObject] = NullableArraySet.Remove(previousLinker, linkedObject);
                    }
                }
                else
                {
                    var linkedArray = ((IEnumerable<DynamicObject>) linked)?.ToArray() ?? Array.Empty<DynamicObject>();

                    var previousLinkedArray = (DynamicObject[]) previousLinked ?? Array.Empty<DynamicObject>();

                    // Use Diff (Add/Remove)
                    var addLinkedArray = linkedArray.Except(previousLinkedArray);
                    var removeLinkedArray = previousLinkedArray.Except(linkedArray);

                    foreach (var addLinked in addLinkedArray)
                    {
                        this.AddLinked(linker, linkedType, addLinked);
                    }

                    foreach (var removeLinked in removeLinkedArray)
                    {
                        this.RemoveLinked(linker, linkedType, removeLinked);
                    }
                }
            }
        }

        internal void AddLinked(DynamicObject linker, DynamicLinkedType linkedType, DynamicObject linked)
        {
            var linkerType = linkedType.LinkerType;
            this.GetLinker(linked, linkerType, out object previousLinker);

            // Linked
            this.GetLinked(linker, linkedType, out var previousLinked);
            var linkedArray = (DynamicObject[]) previousLinked;
            linkedArray = NullableArraySet.Add(linkedArray, linked);

            var changedLinkedByLinker = GetChangedLinkedByLinker(linkedType);
            changedLinkedByLinker[linker] = linkedArray;

            // Linker
            if (linkerType.IsOne)
            {
                // One to Many
                var previousLinkerObject = (DynamicObject) previousLinker;
                if (previousLinkerObject != null)
                {
                    this.GetLinked(previousLinkerObject, linkedType, out var previousLinkerLinked);
                    changedLinkedByLinker[previousLinkerObject] = NullableArraySet.Remove(previousLinkerLinked, linked);
                }

                var changedLinkerByLinked = GetChangedLinkerByLinked(linkerType);
                changedLinkerByLinked[linked] = linker;
            }
            else
            {
                // Many to Many
                var linkerArray = NullableArraySet.Add(previousLinker, linker);
                var changedLinkerByLinked = GetChangedLinkerByLinked(linkerType);
                changedLinkerByLinked[linked] = linkerArray;
            }
        }

        internal void RemoveLinked(DynamicObject linker, DynamicLinkedType linkedType, DynamicObject linked)
        {
            var linkerType = linkedType.LinkerType;
            this.GetLinker(linked, linkerType, out object previousLinker);

            // Linked
            this.GetLinked(linker, linkedType, out var previousLinked);
            if (previousLinked != null)
            {
                var changedLinkedByLinker = GetChangedLinkedByLinker(linkedType);
                changedLinkedByLinker[linker] = NullableArraySet.Remove(previousLinked, linked);

                // Linker
                var changedLinkerByLinked = GetChangedLinkerByLinked(linkerType);
                if (linkerType.IsOne)
                {
                    // One to Many
                    changedLinkerByLinked[linked] = null;
                }
                else
                {
                    // Many to Many
                    changedLinkerByLinked[linked] = NullableArraySet.Add(previousLinker, linker);
                }
            }
        }

        internal void GetLinker(DynamicObject linked, DynamicLinkerType linkerType, out object linker)
        {
            if (this.changedLinkerByLinkedByLinkerType.TryGetValue(linkerType, out var changedLinkerByLinked) &&
                changedLinkerByLinked.TryGetValue(linked, out linker))
            {
                return;
            }

            var linkerByLinked = GetLinkerByLinked(linkerType);
            linkerByLinked.TryGetValue(linked, out linker);
        }

        private Dictionary<DynamicObject, object> GetLinkerByLinked(DynamicLinkerType linkerType)
        {
            if (!this.linkingByLinkerByLinkingType.TryGetValue(linkerType, out var linkerByLinked))
            {
                linkerByLinked = new Dictionary<DynamicObject, object>();
                this.linkingByLinkerByLinkingType[linkerType] = linkerByLinked;
            }

            return linkerByLinked;
        }

        private Dictionary<DynamicObject, object> GetlinkedByLinker(DynamicLinkedType linkedType)
        {
            if (!this.linkedByLinkerByLinkedType.TryGetValue(linkedType, out var linkedByLinker))
            {
                linkedByLinker = new Dictionary<DynamicObject, object>();
                this.linkedByLinkerByLinkedType[linkedType] = linkedByLinker;
            }

            return linkedByLinker;
        }

        private Dictionary<DynamicObject, object> GetChangedLinkerByLinked(DynamicLinkerType linkerType)
        {
            if (!this.changedLinkerByLinkedByLinkerType.TryGetValue(linkerType, out var changedLinkerByLinked))
            {
                changedLinkerByLinked = new Dictionary<DynamicObject, object>();
                this.changedLinkerByLinkedByLinkerType[linkerType] = changedLinkerByLinked;
            }

            return changedLinkerByLinked;
        }

        private Dictionary<DynamicObject, object> GetChangedLinkedByLinker(DynamicLinkedType linkedType)
        {
            if (!this.changedLinkedByLinkerByLinkedType.TryGetValue(linkedType, out var changedLinkedByLinker))
            {
                changedLinkedByLinker = new Dictionary<DynamicObject, object>();
                this.changedLinkedByLinkerByLinkedType[linkedType] = changedLinkedByLinker;
            }

            return changedLinkedByLinker;
        }
    }
}