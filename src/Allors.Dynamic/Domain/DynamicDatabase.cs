using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Allors.Dynamic.Domain.Indexing;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Domain
{
    public sealed class DynamicDatabase
    {
        private readonly DynamicMeta meta;

        private readonly Dictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType;
        private readonly Dictionary<IDynamicCompositeAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType;

        private Dictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> changedRoleByAssociationByRoleType;
        private Dictionary<IDynamicCompositeAssociationType, Dictionary<DynamicObject, object>> changedAssociationByRoleByAssociationType;

        internal DynamicDatabase(DynamicMeta meta)
        {
            this.meta = meta;

            roleByAssociationByRoleType = [];
            associationByRoleByAssociationType = [];

            changedRoleByAssociationByRoleType =
                [];
            changedAssociationByRoleByAssociationType =
                [];
        }

        public IReadOnlyList<DynamicObject> Objects { get; private set; }

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
                                   (roleType.IsMany && Same(originalRole, role));

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
                                   (associationType.IsMany && Same(originalAssociation, changedAssociation));

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

        public void AddObject(DynamicObject newObject)
        {
            Objects = Add(Objects, newObject);
        }

        public object GetRole(DynamicObject association, IDynamicRoleType roleType)
        {
            if (changedRoleByAssociationByRoleType.TryGetValue(roleType, out var changedRoleByAssociation) &&
                changedRoleByAssociation.TryGetValue(association, out var role))
            {
                return role;
            }

            RoleByAssociation(roleType).TryGetValue(association, out role);
            return role;
        }

        public void SetRole(DynamicObject association, IDynamicRoleType roleType, object role)
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
                var associationType = (IDynamicCompositeAssociationType)roleType.AssociationType;
                var previousRole = this.GetRole(association, roleType);
                if (roleType.IsOne)
                {
                    var roleObject = (DynamicObject)normalizedRole;
                    var previousAssociation = GetAssociation(roleObject, associationType);

                    // Role
                    var changedRoleByAssociation = ChangedRoleByAssociation(roleType);
                    changedRoleByAssociation[association] = roleObject;

                    // Association
                    var changedAssociationByRole = ChangedAssociationByRole(associationType);
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

                        changedAssociationByRole[roleObject] = association;
                    }
                    else
                    {
                        changedAssociationByRole[roleObject] = Remove(previousAssociation, roleObject);
                    }
                }
                else
                {
                    var roles = ((IEnumerable)normalizedRole)?.Cast<DynamicObject>().ToArray() ?? Array.Empty<DynamicObject>();
                    var previousRoles = (IReadOnlyList<DynamicObject>)previousRole ?? Array.Empty<DynamicObject>();

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

        public void AddRole(DynamicObject association, IDynamicRoleType roleType, DynamicObject role)
        {
            var associationType = (IDynamicCompositeAssociationType)roleType.AssociationType;
            var previousAssociation = GetAssociation(role, associationType);

            // Role
            var changedRoleByAssociation = ChangedRoleByAssociation(roleType);
            var previousRole = GetRole(association, roleType);
            var roleArray = (IReadOnlyList<DynamicObject>)previousRole;
            roleArray = Add(roleArray, role);
            changedRoleByAssociation[association] = roleArray;

            // Association
            var changedAssociationByRole = ChangedAssociationByRole(associationType);
            if (associationType.IsOne)
            {
                // One to Many
                var previousAssociationObject = (DynamicObject)previousAssociation;
                if (previousAssociationObject != null)
                {
                    var previousAssociationRole = GetRole(previousAssociationObject, roleType);
                    changedRoleByAssociation[previousAssociationObject] = Remove(previousAssociationRole, role);
                }

                changedAssociationByRole[role] = association;
            }
            else
            {
                // Many to Many
                changedAssociationByRole[role] = Add(previousAssociation, association);
            }
        }

        public void RemoveRole(DynamicObject association, IDynamicRoleType roleType, DynamicObject role)
        {
            var associationType = (IDynamicCompositeAssociationType)roleType.AssociationType;
            var previousAssociation = GetAssociation(role, associationType);

            var previousRole = GetRole(association, roleType);
            if (previousRole != null)
            {
                // Role
                var changedRoleByAssociation = ChangedRoleByAssociation(roleType);
                changedRoleByAssociation[association] = Remove(previousRole, role);

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
                    changedAssociationByRole[role] = Remove(previousAssociation, association);
                }
            }
        }

        public void RemoveRole(DynamicObject association, IDynamicRoleType roleType)
        {
            throw new NotImplementedException();
        }

        public object GetAssociation(DynamicObject role, IDynamicCompositeAssociationType associationType)
        {
            if (changedAssociationByRoleByAssociationType.TryGetValue(associationType, out var changedAssociationByRole) &&
                changedAssociationByRole.TryGetValue(role, out var association))
            {
                return association;
            }

            AssociationByRole(associationType).TryGetValue(role, out association);
            return association;
        }

        private Dictionary<DynamicObject, object> AssociationByRole(IDynamicCompositeAssociationType associationType)
        {
            if (!associationByRoleByAssociationType.TryGetValue(associationType, out var associationByRole))
            {
                associationByRole = [];
                associationByRoleByAssociationType[associationType] = associationByRole;
            }

            return associationByRole;
        }

        private Dictionary<DynamicObject, object> RoleByAssociation(IDynamicRoleType roleType)
        {
            if (!roleByAssociationByRoleType.TryGetValue(roleType, out var roleByAssociation))
            {
                roleByAssociation = [];
                roleByAssociationByRoleType[roleType] = roleByAssociation;
            }

            return roleByAssociation;
        }

        private Dictionary<DynamicObject, object> ChangedAssociationByRole(IDynamicCompositeAssociationType associationType)
        {
            if (!changedAssociationByRoleByAssociationType.TryGetValue(associationType, out var changedAssociationByRole))
            {
                changedAssociationByRole = [];
                changedAssociationByRoleByAssociationType[associationType] = changedAssociationByRole;
            }

            return changedAssociationByRole;
        }

        private Dictionary<DynamicObject, object> ChangedRoleByAssociation(IDynamicRoleType roleType)
        {
            if (!changedRoleByAssociationByRoleType.TryGetValue(roleType, out var changedRoleByAssociation))
            {
                changedRoleByAssociation = [];
                changedRoleByAssociationByRoleType[roleType] = changedRoleByAssociation;
            }

            return changedRoleByAssociation;
        }

        private bool Same(object source, object destination)
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

        private IReadOnlyList<DynamicObject> Add(object set, DynamicObject item)
        {
            var objects = DynamicObjects.Ensure(set);
            return objects.Add(item);
        }

        private IReadOnlyList<DynamicObject> Remove(object set, DynamicObject item)
        {
            var objects = DynamicObjects.Ensure(set);
            return objects.Remove(item);
        }
    }
}