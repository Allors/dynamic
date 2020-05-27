using Allors.Dynamic.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Allors.Dynamic
{
    internal class DynamicDatabase
    {
        private readonly DynamicMeta meta;

        private readonly HashSet<DynamicObject> objects;

        private readonly Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByType;
        private readonly Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByType;

        private Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> changedRoleByAssociationByType;
        private Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> changedAssociationByRoleByType;

        internal DynamicDatabase(DynamicMeta meta)
        {
            this.meta = meta;

            this.objects = new HashSet<DynamicObject>();

            this.roleByAssociationByType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.associationByRoleByType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            this.changedRoleByAssociationByType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();
        }

        internal void AddObject(DynamicObject newObject)
        {
            this.objects.Add(newObject);
        }

        internal void GetRole(DynamicObject obj, DynamicRoleType roleType, out object result)
        {
            if (this.changedRoleByAssociationByType.TryGetValue(roleType, out var changeRoleByAssociation) &&
                changeRoleByAssociation.TryGetValue(obj, out result))
            {
                return;
            }

            var roleByAssociation = GetRoleByAssociation(roleType);
            roleByAssociation.TryGetValue(obj, out result);
        }

        internal DynamicChangeSet Snapshot()
        {
            foreach (var roleType in this.changedRoleByAssociationByType.Keys.ToArray())
            {
                var changedRoleByAssociation = this.changedRoleByAssociationByType[roleType];

                var roleByAssociation = GetRoleByAssociation(roleType);
                foreach (var association in changedRoleByAssociation.Keys.ToArray())
                {
                    var changedRole = changedRoleByAssociation[association];
                    roleByAssociation.TryGetValue(association, out var originalRole);

                    var areEqual = ReferenceEquals(originalRole, changedRole) ||
                        (roleType.IsOne && Equals(originalRole, changedRole)) ||
                        (roleType.IsMany && ((IStructuralEquatable)originalRole)?.Equals((IStructuralEquatable)changedRole) == true);

                    if (areEqual)
                    {
                        changedRoleByAssociation.Remove(association);
                        continue;
                    }

                    roleByAssociation[association] = changedRole;
                }

                if (roleByAssociation.Count == 0)
                {
                    this.changedRoleByAssociationByType.Remove(roleType);
                }
            }

            foreach (var associationType in this.changedAssociationByRoleByType.Keys.ToArray())
            {
                var changedAssociationByRole = this.changedAssociationByRoleByType[associationType];

                var associationByRole = GetAssociationByRole(associationType);
                foreach (var role in changedAssociationByRole.Keys.ToArray())
                {
                    var changedAssociation = changedAssociationByRole[role];
                    associationByRole.TryGetValue(role, out var originalAssociation);

                    var areEqual = ReferenceEquals(originalAssociation, changedAssociation) ||
                      (associationType.IsOne && Equals(originalAssociation, changedAssociation)) ||
                      (associationType.IsMany && ((IStructuralEquatable)originalAssociation)?.Equals((IStructuralEquatable)changedAssociation) == true);

                    if (areEqual)
                    {
                        changedAssociationByRole.Remove(role);
                        continue;
                    }

                    associationByRole[role] = changedAssociation;
                }

                if (associationByRole.Count == 0)
                {
                    this.changedAssociationByRoleByType.Remove(associationType);
                }
            }

            var snapshot = new DynamicChangeSet(this.meta, this.changedRoleByAssociationByType, this.changedAssociationByRoleByType);

            this.changedRoleByAssociationByType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            return snapshot;
        }

        internal void AddRole(DynamicObject association, DynamicRoleType roleType, DynamicObject role)
        {
            var associationType = roleType.AssociationType;
            this.GetAssociation(role, associationType, out object previousAssociation);

            // Role
            this.GetRole(association, roleType, out var previousRole);
            var roleArray = (DynamicObject[])previousRole;
            if (previousRole == null)
            {
                roleArray = new DynamicObject[] { role };
            }
            else
            {
                if (!roleArray.Contains(role))
                {
                    Array.Resize(ref roleArray, roleArray.Length + 1);
                    roleArray[roleArray.Length - 1] = role;
                }
            }

            var changedRoleByAssociation = GetChangedRoleByAssociation(roleType);
            changedRoleByAssociation[association] = roleArray;

            // Association
            if (associationType.IsOne)
            {
                // One to Many
                var previousAssociationObject = (DynamicObject)previousAssociation;
                if (previousAssociationObject != null)
                {
                    this.GetRole(previousAssociationObject, roleType, out var x);
                    var array = x as DynamicObject[];
                    var index = Array.IndexOf(array, role);
                    array[index] = array[array.Length - 1];
                    Array.Resize(ref array, array.Length - 1);
                    changedRoleByAssociation[previousAssociationObject] = array;
                }

                var changedAssociationByRole = GetChangedAssociationByRole(associationType);
                changedAssociationByRole[role] = association;
            }
            else
            {
                // Many to Many
                var associationArray = (DynamicObject[])previousAssociation;
                if (associationArray == null || associationArray.Length == 0)
                {
                    associationArray = new DynamicObject[] { association };
                }
                else
                {
                    if (!associationArray.Contains(association))
                    {
                        Array.Resize(ref associationArray, associationArray.Length + 1);
                        associationArray[associationArray.Length - 1] = association;
                    }
                }

                var changedAssociationByRole = GetChangedAssociationByRole(associationType);
                changedAssociationByRole[role] = associationArray;
            }
        }

        internal void RemoveRole(DynamicObject association, DynamicRoleType roleType, DynamicObject role)
        {
            var associationType = roleType.AssociationType;
            this.GetAssociation(role, associationType, out object previousAssociation);

            // Role
            this.GetRole(association, roleType, out var previousRole);
            if (previousRole != null)
            {
                var changedRoleByAssociation = GetChangedRoleByAssociation(roleType);

                var previousRoleArray = previousRole as DynamicObject[];
                var index = Array.IndexOf(previousRoleArray, role);
                if (index > -1)
                {
                    previousRoleArray[index] = previousRoleArray[previousRoleArray.Length - 1];
                    Array.Resize(ref previousRoleArray, previousRoleArray.Length - 1);
                    changedRoleByAssociation[association] = previousRoleArray;
                }

                // Association
                var changedAssociationByRole = GetChangedAssociationByRole(associationType);
                if (associationType.IsOne)
                {
                    // One to Many
                    changedAssociationByRole[role] = null;
                }
                else
                {
                    // Many to Many
                    var associationArray = (DynamicObject[])previousAssociation;
                    Array.Resize(ref associationArray, associationArray.Length + 1);
                    associationArray[associationArray.Length - 1] = association;
                    changedAssociationByRole[role] = associationArray;
                }
            }
        }

        internal void SetRole(dynamic association, DynamicRoleType roleType, object value)
        {
            if (roleType.IsUnit)
            {
                // Role
                var changedRoleByAssociation = GetChangedRoleByAssociation(roleType);
                changedRoleByAssociation[association] = value;
            }
            else
            {
                var associationType = roleType.AssociationType;

                if (roleType.IsOne)
                {
                    var role = (DynamicObject)value;
                    this.GetAssociation(role, associationType, out object previousAssociation);

                    // Role
                    var changedRoleByAssociation = GetChangedRoleByAssociation(roleType);
                    changedRoleByAssociation[association] = role;

                    // Association
                    var changedAssociationByRole = GetChangedAssociationByRole(associationType);
                    if (associationType.IsOne)
                    {
                        // One to One
                        var previousAssociationObject = (DynamicObject)previousAssociation;
                        if (previousAssociationObject != null)
                        {
                            changedRoleByAssociation[previousAssociationObject] = null;
                        }

                        changedAssociationByRole[role] = association;
                    }
                    else
                    {
                        // Many to One
                        var previousAssociationArray = (DynamicObject[])previousAssociation;
                        var index = Array.IndexOf(previousAssociationArray, role);
                        previousAssociationArray[index] = previousAssociationArray[previousAssociationArray.Length - 1];
                        Array.Resize(ref previousAssociationArray, previousAssociationArray.Length - 1);
                        changedAssociationByRole[role] = previousAssociationArray;
                    }
                }
                else
                {
                    var roleArray = ((IEnumerable<DynamicObject>)value).ToArray() ?? Array.Empty<DynamicObject>();
                    this.GetRole(association, roleType, out object previousRole);
                    var previousRoleArray = (DynamicObject[])previousRole ?? Array.Empty<DynamicObject>();

                    // Use Diff (Add/Remove)
                    var toAdd = roleArray.Except(previousRoleArray);
                    var toRemove = previousRoleArray.Except(roleArray);

                    foreach(var role in toAdd)
                    {
                        this.AddRole(association, roleType, role);
                    }

                    foreach (var role in toRemove)
                    {
                        this.RemoveRole(association, roleType, role);
                    }
                }
            }
        }

        internal void GetAssociation(DynamicObject obj, DynamicAssociationType associationType, out object result)
        {
            if (this.changedAssociationByRoleByType.TryGetValue(associationType, out var changedAssociationByRole) &&
                changedAssociationByRole.TryGetValue(obj, out result))
            {
                return;
            }

            var associationByRole = GetAssociationByRole(associationType);
            associationByRole.TryGetValue(obj, out result);
        }

        private Dictionary<DynamicObject, object> GetAssociationByRole(DynamicAssociationType associationType)
        {
            // Lazy create associationByRole
            if (!this.associationByRoleByType.TryGetValue(associationType, out var associationByRole))
            {
                associationByRole = new Dictionary<DynamicObject, object>();
                this.associationByRoleByType[associationType] = associationByRole;
            }

            return associationByRole;
        }

        private Dictionary<DynamicObject, object> GetRoleByAssociation(DynamicRoleType roleType)
        {
            // Lazy create roleByAssociation
            if (!this.roleByAssociationByType.TryGetValue(roleType, out var roleByAssociation))
            {
                roleByAssociation = new Dictionary<DynamicObject, object>();
                this.roleByAssociationByType[roleType] = roleByAssociation;
            }

            return roleByAssociation;
        }

        private Dictionary<DynamicObject, object> GetChangedAssociationByRole(DynamicAssociationType associationType)
        {
            // Lazy create associationByRole
            if (!this.changedAssociationByRoleByType.TryGetValue(associationType, out var changedAssociationByRole))
            {
                changedAssociationByRole = new Dictionary<DynamicObject, object>();
                this.changedAssociationByRoleByType[associationType] = changedAssociationByRole;
            }

            return changedAssociationByRole;
        }

        private Dictionary<DynamicObject, object> GetChangedRoleByAssociation(DynamicRoleType roleType)
        {
            // Lazy create roleByAssociation
            if (!this.changedRoleByAssociationByType.TryGetValue(roleType, out var changedRoleByAssociation))
            {
                changedRoleByAssociation = new Dictionary<DynamicObject, object>();
                this.changedRoleByAssociationByType[roleType] = changedRoleByAssociation;
            }

            return changedRoleByAssociation;
        }
    }
}
