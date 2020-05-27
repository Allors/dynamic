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

        private readonly Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType;
        private readonly Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType;

        private Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> changedRoleByAssociationByRoleType;
        private Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> changedAssociationByRoleByRoleType;

        internal DynamicDatabase(DynamicMeta meta)
        {
            this.meta = meta;

            this.objects = new HashSet<DynamicObject>();

            this.roleByAssociationByRoleType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.associationByRoleByAssociationType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            this.changedRoleByAssociationByRoleType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByRoleType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();
        }

        internal void AddObject(DynamicObject newObject)
        {
            this.objects.Add(newObject);
        }

        internal void GetRole(DynamicObject obj, DynamicRoleType roleType, out object result)
        {
            if (this.changedRoleByAssociationByRoleType.TryGetValue(roleType, out var changeRoleByAssociation) &&
                changeRoleByAssociation.TryGetValue(obj, out result))
            {
                return;
            }

            var roleByAssociation = GetRoleByAssociation(roleType);
            roleByAssociation.TryGetValue(obj, out result);
        }

        internal DynamicChangeSet Snapshot()
        {
            foreach (var roleType in this.changedRoleByAssociationByRoleType.Keys.ToArray())
            {
                var changedRoleByAssociation = this.changedRoleByAssociationByRoleType[roleType];

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
                    this.changedRoleByAssociationByRoleType.Remove(roleType);
                }
            }

            foreach (var associationType in this.changedAssociationByRoleByRoleType.Keys.ToArray())
            {
                var changedAssociationByRole = this.changedAssociationByRoleByRoleType[associationType];

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
                    this.changedAssociationByRoleByRoleType.Remove(associationType);
                }
            }

            var snapshot = new DynamicChangeSet(this.meta, this.changedRoleByAssociationByRoleType, this.changedAssociationByRoleByRoleType);

            this.changedRoleByAssociationByRoleType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByRoleType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            return snapshot;
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

                this.GetRole(association, roleType, out object previousRole);

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

                        if (previousRole != null)
                        {
                            var previousRoleObject = (DynamicObject)previousRole;
                            changedAssociationByRole[previousRoleObject] = null;
                        }

                        changedAssociationByRole[role] = association;
                    }
                    else
                    {
                        changedAssociationByRole[role] = NullableArraySet.Remove(previousAssociation, role);
                    }
                }
                else
                {
                    var roleArray = ((IEnumerable<DynamicObject>)value).ToArray() ?? Array.Empty<DynamicObject>();

                    var previousRoleArray = (DynamicObject[])previousRole ?? Array.Empty<DynamicObject>();

                    // Use Diff (Add/Remove)
                    var toAdd = roleArray.Except(previousRoleArray);
                    var toRemove = previousRoleArray.Except(roleArray);

                    foreach (var role in toAdd)
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
                roleArray = NullableArraySet.Add(roleArray, role);
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
                    this.GetRole(previousAssociationObject, roleType, out var previousAssociationRole);
                    changedRoleByAssociation[previousAssociationObject] = NullableArraySet.Remove(previousAssociationRole, role);
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
                    associationArray = NullableArraySet.Add(associationArray, association);
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
                changedRoleByAssociation[association] = NullableArraySet.Remove(previousRole, role);

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
                    changedAssociationByRole[role] = NullableArraySet.Add(previousAssociation, association);
                }
            }
        }

        internal void GetAssociation(DynamicObject obj, DynamicAssociationType associationType, out object result)
        {
            if (this.changedAssociationByRoleByRoleType.TryGetValue(associationType, out var changedAssociationByRole) &&
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
            if (!this.associationByRoleByAssociationType.TryGetValue(associationType, out var associationByRole))
            {
                associationByRole = new Dictionary<DynamicObject, object>();
                this.associationByRoleByAssociationType[associationType] = associationByRole;
            }

            return associationByRole;
        }

        private Dictionary<DynamicObject, object> GetRoleByAssociation(DynamicRoleType roleType)
        {
            // Lazy create roleByAssociation
            if (!this.roleByAssociationByRoleType.TryGetValue(roleType, out var roleByAssociation))
            {
                roleByAssociation = new Dictionary<DynamicObject, object>();
                this.roleByAssociationByRoleType[roleType] = roleByAssociation;
            }

            return roleByAssociation;
        }

        private Dictionary<DynamicObject, object> GetChangedAssociationByRole(DynamicAssociationType associationType)
        {
            // Lazy create associationByRole
            if (!this.changedAssociationByRoleByRoleType.TryGetValue(associationType, out var changedAssociationByRole))
            {
                changedAssociationByRole = new Dictionary<DynamicObject, object>();
                this.changedAssociationByRoleByRoleType[associationType] = changedAssociationByRole;
            }

            return changedAssociationByRole;
        }

        private Dictionary<DynamicObject, object> GetChangedRoleByAssociation(DynamicRoleType roleType)
        {
            // Lazy create roleByAssociation
            if (!this.changedRoleByAssociationByRoleType.TryGetValue(roleType, out var changedRoleByAssociation))
            {
                changedRoleByAssociation = new Dictionary<DynamicObject, object>();
                this.changedRoleByAssociationByRoleType[roleType] = changedRoleByAssociation;
            }

            return changedRoleByAssociation;
        }
    }
}
