using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic
{
    public sealed class DynamicDatabase
    {
        private readonly DynamicMeta meta;

        private readonly Dictionary<IDynamicRoleType, Dictionary<IDynamicObject, object>> roleByAssociationByRoleType;
        private readonly Dictionary<IDynamicAssociationType, Dictionary<IDynamicObject, object>> associationByRoleByAssociationType;

        private Dictionary<IDynamicRoleType, Dictionary<IDynamicObject, object>> changedRoleByAssociationByRoleType;
        private Dictionary<IDynamicAssociationType, Dictionary<IDynamicObject, object>> changedAssociationByRoleByAssociationType;

        public DynamicDatabase(DynamicMeta meta)
        {
            this.meta = meta;

            roleByAssociationByRoleType = [];
            associationByRoleByAssociationType = [];

            changedRoleByAssociationByRoleType =
                [];
            changedAssociationByRoleByAssociationType =
                [];
        }

        public IDynamicObject[] Objects { get; private set; }

        public DynamicChangeSet Snapshot()
        {
            foreach (var roleType in changedRoleByAssociationByRoleType.Keys.ToArray())
            {
                var changedRoleByAssociation = changedRoleByAssociationByRoleType[roleType];
                var roleByAssociation = RoleByAssociation(roleType);

                foreach (var association in changedRoleByAssociation.Keys.ToArray())
                {
                    var role = changedRoleByAssociation[association];
                    roleByAssociation.TryGetValue(association, out var originalRole);

                    var areEqual = ReferenceEquals(originalRole, role) ||
                                   (roleType.IsOne && Equals(originalRole, role)) ||
                                   (roleType.IsMany && NullableArraySet.Same(originalRole, role));

                    if (areEqual)
                    {
                        changedRoleByAssociation.Remove(association);
                        continue;
                    }

                    roleByAssociation[association] = role;
                }

                if (roleByAssociation.Count == 0)
                {
                    changedRoleByAssociationByRoleType.Remove(roleType);
                }
            }

            foreach (var associationType in changedAssociationByRoleByAssociationType.Keys.ToArray())
            {
                var changedAssociationByRole = changedAssociationByRoleByAssociationType[associationType];
                var associationByRole = AssociationByRole(associationType);

                foreach (var role in changedAssociationByRole.Keys.ToArray())
                {
                    var changedAssociation = changedAssociationByRole[role];
                    associationByRole.TryGetValue(role, out var originalAssociation);

                    var areEqual = ReferenceEquals(originalAssociation, changedAssociation) ||
                                   (associationType.IsOne && Equals(originalAssociation, changedAssociation)) ||
                                   (associationType.IsMany && NullableArraySet.Same(originalAssociation, changedAssociation));

                    if (areEqual)
                    {
                        changedAssociationByRole.Remove(role);
                        continue;
                    }

                    associationByRole[role] = changedAssociation;
                }

                if (associationByRole.Count == 0)
                {
                    changedAssociationByRoleByAssociationType.Remove(associationType);
                }
            }

            var snapshot = new DynamicChangeSet(meta, changedRoleByAssociationByRoleType, changedAssociationByRoleByAssociationType);

            foreach (var kvp in changedRoleByAssociationByRoleType)
            {
                var roleType = kvp.Key;
                var changedRoleByAssociation = kvp.Value;

                var roleByAssociation = RoleByAssociation(roleType);

                foreach (var kvp2 in changedRoleByAssociation)
                {
                    var association = kvp2.Key;
                    var changedRole = kvp2.Value;
                    roleByAssociation[association] = changedRole;
                }
            }

            foreach (var kvp in changedAssociationByRoleByAssociationType)
            {
                var associationType = kvp.Key;
                var changedAssociationByRole = kvp.Value;

                var associationByRole = AssociationByRole(associationType);

                foreach (var kvp2 in changedAssociationByRole)
                {
                    var role = kvp2.Key;
                    var changedAssociation = kvp2.Value;
                    associationByRole[role] = changedAssociation;
                }
            }

            changedRoleByAssociationByRoleType = [];
            changedAssociationByRoleByAssociationType = [];

            return snapshot;
        }

        public void AddObject(IDynamicObject newObject)
        {
            Objects = NullableArraySet.Add(Objects, newObject);
        }

        public void GetRole(IDynamicObject association, IDynamicRoleType roleType, out object role)
        {
            if (changedRoleByAssociationByRoleType.TryGetValue(roleType, out var changedRoleByAssociation) &&
                changedRoleByAssociation.TryGetValue(association, out role))
            {
                return;
            }

            RoleByAssociation(roleType).TryGetValue(association, out role);
        }

        public void SetRole(IDynamicObject association, IDynamicRoleType roleType, object role)
        {
            if (role == null)
            {
                this.RemoveRole(association, roleType);
                return;
            }

            var normalizedRole = roleType.Normalize(role);

            if (roleType.IsUnit)
            {
                // Role
                ChangedRoleByAssociation(roleType)[association] = normalizedRole;
            }
            else
            {
                var associationType = roleType.AssociationType;
                this.GetRole(association, roleType, out object previousRole);
                if (roleType.IsOne)
                {
                    var roleObject = (IDynamicObject)normalizedRole;
                    GetAssociation(roleObject, associationType, out var previousAssociation);

                    // Role
                    var changedRoleByAssociation = ChangedRoleByAssociation(roleType);
                    changedRoleByAssociation[association] = roleObject;

                    // Association
                    var changedAssociationByRole = ChangedAssociationByRole(associationType);
                    if (associationType.IsOne)
                    {
                        // One to One
                        var previousAssociationObject = (IDynamicObject)previousAssociation;
                        if (previousAssociationObject != null)
                        {
                            changedRoleByAssociation[previousAssociationObject] = null;
                        }

                        if (previousRole != null)
                        {
                            var previousRoleObject = (IDynamicObject)previousRole;
                            changedAssociationByRole[previousRoleObject] = null;
                        }

                        changedAssociationByRole[roleObject] = association;
                    }
                    else
                    {
                        changedAssociationByRole[roleObject] = NullableArraySet.Remove(previousAssociation, roleObject);
                    }
                }
                else
                {
                    var roles = ((IEnumerable)normalizedRole)?.Cast<IDynamicObject>().ToArray() ?? Array.Empty<IDynamicObject>();
                    var previousRoles = (IDynamicObject[])previousRole ?? Array.Empty<IDynamicObject>();

                    // Use Diff (Add/Remove)
                    var addedRoles = roles.Except(previousRoles);
                    var removedRoles = previousRoles.Except(roles);

                    foreach (var addedRole in addedRoles)
                    {
                        this.AddRole(association, roleType, addedRole);
                    }

                    foreach (var removeRole in removedRoles)
                    {
                        this.RemoveRole(association, roleType, removeRole);
                    }
                }
            }
        }

        public void AddRole(IDynamicObject association, IDynamicRoleType roleType, IDynamicObject role)
        {
            var associationType = roleType.AssociationType;
            GetAssociation(role, associationType, out var previousAssociation);

            // Role
            var changedRoleByAssociation = ChangedRoleByAssociation(roleType);
            GetRole(association, roleType, out var previousRole);
            var roleArray = (IDynamicObject[])previousRole;
            roleArray = NullableArraySet.Add(roleArray, role);
            changedRoleByAssociation[association] = roleArray;

            // Association
            var changedAssociationByRole = ChangedAssociationByRole(associationType);
            if (associationType.IsOne)
            {
                // One to Many
                var previousAssociationObject = (IDynamicObject)previousAssociation;
                if (previousAssociationObject != null)
                {
                    GetRole(previousAssociationObject, roleType, out var previousAssociationRole);
                    changedRoleByAssociation[previousAssociationObject] = NullableArraySet.Remove(previousAssociationRole, role);
                }

                changedAssociationByRole[role] = association;
            }
            else
            {
                // Many to Many
                changedAssociationByRole[role] = NullableArraySet.Add(previousAssociation, association);
            }
        }

        public void RemoveRole(IDynamicObject association, IDynamicRoleType roleType, IDynamicObject role)
        {
            var associationType = roleType.AssociationType;
            GetAssociation(role, associationType, out var previousAssociation);

            GetRole(association, roleType, out var previousRole);
            if (previousRole != null)
            {
                // Role
                var changedRoleByAssociation = ChangedRoleByAssociation(roleType);
                changedRoleByAssociation[association] = NullableArraySet.Remove(previousRole, role);

                // Association
                var changedAssociationByRole = ChangedAssociationByRole(associationType);
                if (associationType.IsOne)
                {
                    // One to Many
                    changedAssociationByRole[role] = null;
                }
                else
                {
                    // Many to Many
                    changedAssociationByRole[role] = NullableArraySet.Remove(previousAssociation, association);
                }
            }
        }

        public void RemoveRole(IDynamicObject association, IDynamicRoleType roleType)
        {
            throw new NotImplementedException();
        }

        public void GetAssociation(IDynamicObject role, IDynamicAssociationType associationType, out object association)
        {
            if (changedAssociationByRoleByAssociationType.TryGetValue(associationType, out var changedAssociationByRole) &&
                changedAssociationByRole.TryGetValue(role, out association))
            {
                return;
            }

            AssociationByRole(associationType).TryGetValue(role, out association);
        }

        private Dictionary<IDynamicObject, object> AssociationByRole(IDynamicAssociationType asscociationType)
        {
            if (!associationByRoleByAssociationType.TryGetValue(asscociationType, out var associationByRole))
            {
                associationByRole = [];
                associationByRoleByAssociationType[asscociationType] = associationByRole;
            }

            return associationByRole;
        }

        private Dictionary<IDynamicObject, object> RoleByAssociation(IDynamicRoleType roleType)
        {
            if (!roleByAssociationByRoleType.TryGetValue(roleType, out var roleByAssociation))
            {
                roleByAssociation = [];
                roleByAssociationByRoleType[roleType] = roleByAssociation;
            }

            return roleByAssociation;
        }

        private Dictionary<IDynamicObject, object> ChangedAssociationByRole(IDynamicAssociationType associationType)
        {
            if (!changedAssociationByRoleByAssociationType.TryGetValue(associationType, out var changedAssociationByRole))
            {
                changedAssociationByRole = [];
                changedAssociationByRoleByAssociationType[associationType] = changedAssociationByRole;
            }

            return changedAssociationByRole;
        }

        private Dictionary<IDynamicObject, object> ChangedRoleByAssociation(IDynamicRoleType roleType)
        {
            if (!changedRoleByAssociationByRoleType.TryGetValue(roleType, out var changedRoleByAssociation))
            {
                changedRoleByAssociation = [];
                changedRoleByAssociationByRoleType[roleType] = changedRoleByAssociation;
            }

            return changedRoleByAssociation;
        }
    }
}