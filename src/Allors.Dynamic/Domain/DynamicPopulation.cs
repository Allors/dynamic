namespace Allors.Dynamic.Domain
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Dynamic.Domain.Indexing;
    using Allors.Dynamic.Meta;

    public sealed class DynamicPopulation(DynamicMeta meta)
    {
        private readonly Dictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType = [];
        private readonly Dictionary<IDynamicCompositeAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType = [];

        private Dictionary<IDynamicRoleType, Dictionary<DynamicObject, object>> changedRoleByAssociationByRoleType = [];
        private Dictionary<IDynamicCompositeAssociationType, Dictionary<DynamicObject, object>> changedAssociationByRoleByAssociationType = [];

        public Dictionary<string, IDynamicDerivation> DerivationById { get; } = [];

        public IEnumerable<DynamicObject> Objects { get; private set; } = null!;

        public DynamicObject Create(DynamicObjectType @class, params Action<DynamicObject>[] builders)
        {
            var @new = new DynamicObject(this, @class);
            this.AddObject(@new);

            foreach (var builder in builders)
            {
                builder(@new);
            }

            return @new;
        }

        public DynamicChangeSet Snapshot()
        {
            foreach (var roleType in this.changedRoleByAssociationByRoleType.Keys.ToArray())
            {
                var changedRoleByAssociation = this.changedRoleByAssociationByRoleType[roleType];
                var roleByAssociation = this.RoleByAssociation(roleType);

                foreach (var association in changedRoleByAssociation.Keys.ToArray())
                {
                    var role = changedRoleByAssociation[association];
                    roleByAssociation.TryGetValue(association, out var originalRole);

                    var areEqual = ReferenceEquals(originalRole, role) ||
                                   (roleType.IsOne && Equals(originalRole, role)) ||
                                   (roleType.IsMany && this.Same(originalRole, role));

                    if (areEqual)
                    {
                        changedRoleByAssociation.Remove(association);
                        continue;
                    }

                    roleByAssociation[association] = role;
                }

                if (roleByAssociation.Count == 0)
                {
                    this.changedRoleByAssociationByRoleType.Remove(roleType);
                }
            }

            foreach (var associationType in this.changedAssociationByRoleByAssociationType.Keys.ToArray())
            {
                var changedAssociationByRole = this.changedAssociationByRoleByAssociationType[associationType];
                var associationByRole = this.AssociationByRole(associationType);

                foreach (var role in changedAssociationByRole.Keys.ToArray())
                {
                    var changedAssociation = changedAssociationByRole[role];
                    associationByRole.TryGetValue(role, out var originalAssociation);

                    var areEqual = ReferenceEquals(originalAssociation, changedAssociation) ||
                                   (associationType.IsOne && Equals(originalAssociation, changedAssociation)) ||
                                   (associationType.IsMany && this.Same(originalAssociation, changedAssociation));

                    if (areEqual)
                    {
                        changedAssociationByRole.Remove(role);
                        continue;
                    }

                    associationByRole[role] = changedAssociation;
                }

                if (associationByRole.Count == 0)
                {
                    this.changedAssociationByRoleByAssociationType.Remove(associationType);
                }
            }

            var snapshot = new DynamicChangeSet(meta, this.changedRoleByAssociationByRoleType, this.changedAssociationByRoleByAssociationType);

            foreach (var kvp in this.changedRoleByAssociationByRoleType)
            {
                var roleType = kvp.Key;
                var changedRoleByAssociation = kvp.Value;

                var roleByAssociation = this.RoleByAssociation(roleType);

                foreach (var kvp2 in changedRoleByAssociation)
                {
                    var association = kvp2.Key;
                    var changedRole = kvp2.Value;
                    roleByAssociation[association] = changedRole;
                }
            }

            foreach (var kvp in this.changedAssociationByRoleByAssociationType)
            {
                var associationType = kvp.Key;
                var changedAssociationByRole = kvp.Value;

                var associationByRole = this.AssociationByRole(associationType);

                foreach (var kvp2 in changedAssociationByRole)
                {
                    var role = kvp2.Key;
                    var changedAssociation = kvp2.Value;
                    associationByRole[role] = changedAssociation;
                }
            }

            this.changedRoleByAssociationByRoleType = [];
            this.changedAssociationByRoleByAssociationType = [];

            return snapshot;
        }

        public void Derive()
        {
            var changeSet = this.Snapshot();

            while (changeSet.HasChanges)
            {
                foreach (var kvp in this.DerivationById)
                {
                    var derivation = kvp.Value;
                    derivation.Derive(changeSet);
                }

                changeSet = this.Snapshot();
            }
        }

        internal object? GetRole(DynamicObject association, IDynamicRoleType roleType)
        {
            if (this.changedRoleByAssociationByRoleType.TryGetValue(roleType, out var changedRoleByAssociation) &&
                changedRoleByAssociation.TryGetValue(association, out var role))
            {
                return role;
            }

            this.RoleByAssociation(roleType).TryGetValue(association, out role);
            return role;
        }

        internal void SetUnitRole(DynamicObject association, IDynamicRoleType roleType, object? role)
        {
            var normalizedRole = roleType.Normalize(role);

            if (normalizedRole == null)
            {
                this.RemoveRole(association, roleType);
                return;
            }

            // Role
            this.ChangedRoleByAssociation(roleType)[association] = normalizedRole;
        }

        internal void SetToOneRole(DynamicObject association, IDynamicRoleType roleType, object? role)
        {
            var normalizedRole = roleType.Normalize(role);

            if (normalizedRole == null)
            {
                this.RemoveRole(association, roleType);
                return;
            }

            var associationType = (IDynamicCompositeAssociationType)roleType.AssociationType;
            var previousRole = this.GetRole(association, roleType);

            var roleObject = (DynamicObject)normalizedRole;
            var previousAssociation = this.GetAssociation(roleObject, associationType);

            // Role
            var changedRoleByAssociation = this.ChangedRoleByAssociation(roleType);
            changedRoleByAssociation[association] = roleObject;

            // Association
            var changedAssociationByRole = this.ChangedAssociationByRole(associationType);
            if (associationType.IsOne)
            {
                // One to One
                var previousAssociationObject = (DynamicObject?)previousAssociation;
                if (previousAssociationObject != null)
                {
                    changedRoleByAssociation.Remove(previousAssociationObject);
                }

                if (previousRole != null)
                {
                    var previousRoleObject = (DynamicObject)previousRole;
                    changedAssociationByRole.Remove(previousRoleObject);
                }

                changedAssociationByRole[roleObject] = association;
            }
            else
            {
                changedAssociationByRole[roleObject] = DynamicObjects.Ensure(previousAssociation).Remove(roleObject);
            }
        }

        internal void SetToManyRole(DynamicObject association, IDynamicRoleType roleType, object? role)
        {
            var normalizedRole = roleType.Normalize(role);

            if (normalizedRole == null)
            {
                this.RemoveRole(association, roleType);
                return;
            }

            var associationType = (IDynamicCompositeAssociationType)roleType.AssociationType;
            var previousRole = this.GetRole(association, roleType);

            var roles = ((IEnumerable)normalizedRole)?.Cast<DynamicObject>().ToArray() ?? Array.Empty<DynamicObject>();
            var previousRoles = (IEnumerable<DynamicObject>?)previousRole ?? Array.Empty<DynamicObject>();

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

        internal void AddRole(DynamicObject association, IDynamicRoleType roleType, DynamicObject role)
        {
            var associationType = (IDynamicCompositeAssociationType)roleType.AssociationType;
            var previousAssociation = this.GetAssociation(role, associationType);

            // Role
            var changedRoleByAssociation = this.ChangedRoleByAssociation(roleType);
            var previousRole = this.GetRole(association, roleType);
            var roleArray = (IEnumerable<DynamicObject>?)previousRole;
            roleArray = DynamicObjects.Ensure(roleArray).Add(role);
            changedRoleByAssociation[association] = roleArray;

            // Association
            var changedAssociationByRole = this.ChangedAssociationByRole(associationType);
            if (associationType.IsOne)
            {
                // One to Many
                var previousAssociationObject = (DynamicObject?)previousAssociation;
                if (previousAssociationObject != null)
                {
                    var previousAssociationRole = this.GetRole(previousAssociationObject, roleType);
                    changedRoleByAssociation[previousAssociationObject] = DynamicObjects.Ensure(previousAssociationRole).Remove(role);
                }

                changedAssociationByRole[role] = association;
            }
            else
            {
                // Many to Many
                changedAssociationByRole[role] = DynamicObjects.Ensure(previousAssociation).Add(association);
            }
        }

        internal void RemoveRole(DynamicObject association, IDynamicRoleType roleType, DynamicObject role)
        {
            var associationType = (IDynamicCompositeAssociationType)roleType.AssociationType;
            var previousAssociation = this.GetAssociation(role, associationType);

            var previousRole = this.GetRole(association, roleType);
            if (previousRole != null)
            {
                // Role
                var changedRoleByAssociation = this.ChangedRoleByAssociation(roleType);
                changedRoleByAssociation[association] = DynamicObjects.Ensure(previousRole).Remove(role);

                // Association
                var changedAssociationByRole = this.ChangedAssociationByRole(associationType);
                if (associationType.IsOne)
                {
                    // One to Many
                    changedAssociationByRole.Remove(role);
                }
                else
                {
                    // Many to Many
                    changedAssociationByRole[role] = DynamicObjects.Ensure(previousAssociation).Remove(association);
                }
            }
        }

        internal object? GetAssociation(DynamicObject role, IDynamicCompositeAssociationType associationType)
        {
            if (this.changedAssociationByRoleByAssociationType.TryGetValue(associationType, out var changedAssociationByRole) &&
                changedAssociationByRole.TryGetValue(role, out var association))
            {
                return association;
            }

            this.AssociationByRole(associationType).TryGetValue(role, out association);
            return association;
        }

        private Dictionary<DynamicObject, object> AssociationByRole(IDynamicCompositeAssociationType associationType)
        {
            if (!this.associationByRoleByAssociationType.TryGetValue(associationType, out var associationByRole))
            {
                associationByRole = [];
                this.associationByRoleByAssociationType[associationType] = associationByRole;
            }

            return associationByRole;
        }

        private Dictionary<DynamicObject, object> RoleByAssociation(IDynamicRoleType roleType)
        {
            if (!this.roleByAssociationByRoleType.TryGetValue(roleType, out var roleByAssociation))
            {
                roleByAssociation = [];
                this.roleByAssociationByRoleType[roleType] = roleByAssociation;
            }

            return roleByAssociation;
        }

        private Dictionary<DynamicObject, object> ChangedAssociationByRole(IDynamicCompositeAssociationType associationType)
        {
            if (!this.changedAssociationByRoleByAssociationType.TryGetValue(associationType, out var changedAssociationByRole))
            {
                changedAssociationByRole = [];
                this.changedAssociationByRoleByAssociationType[associationType] = changedAssociationByRole;
            }

            return changedAssociationByRole;
        }

        private Dictionary<DynamicObject, object> ChangedRoleByAssociation(IDynamicRoleType roleType)
        {
            if (!this.changedRoleByAssociationByRoleType.TryGetValue(roleType, out var changedRoleByAssociation))
            {
                changedRoleByAssociation = [];
                this.changedRoleByAssociationByRoleType[roleType] = changedRoleByAssociation;
            }

            return changedRoleByAssociation;
        }

        private void AddObject(DynamicObject newObject)
        {
            this.Objects = DynamicObjects.Ensure(this.Objects).Add(newObject);
        }

        private void RemoveRole(DynamicObject association, IDynamicRoleType roleType)
        {
            throw new NotImplementedException();
        }

        private bool Same(object? source, object? destination)
        {
            if (source == null && destination == null)
            {
                return true;
            }

            if (source == null || destination == null)
            {
                return false;
            }

            if (source is IReadOnlyList<DynamicObject> sourceList &&
                destination is IReadOnlyList<DynamicObject> destinationList &&
                sourceList.Count != destinationList.Count)
            {
                return false;
            }

            var sourceEnumeration = (IEnumerable<DynamicObject>)source;
            var destinationEnumeration = (IEnumerable<DynamicObject>)source;

            return sourceEnumeration.All(v => destinationEnumeration.Contains(v));
        }
    }
}
