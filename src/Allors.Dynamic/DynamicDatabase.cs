namespace Allors.Dynamic
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Dynamic.Meta;

    internal class DynamicDatabase
    {
        private readonly DynamicMeta meta;

        private readonly Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByRoleType;
        private readonly Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByAssociationType;

        private Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> changedRoleByAssociationByRoleType;
        private Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> changedAssociationByRoleByAssociationType;

        internal DynamicDatabase(DynamicMeta meta)
        {
            this.meta = meta;

            this.roleByAssociationByRoleType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.associationByRoleByAssociationType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            this.changedRoleByAssociationByRoleType =
                new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByAssociationType =
                new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();
        }

        internal DynamicObject[] Objects { get; private set; }

        internal DynamicChangeSet Snapshot()
        {
            foreach (DynamicRoleType roleType in this.changedRoleByAssociationByRoleType.Keys.ToArray())
            {
                Dictionary<DynamicObject, object> changedRoleByAssociation = this.changedRoleByAssociationByRoleType[roleType];
                Dictionary<DynamicObject, object> roleByAssociation = this.RoleByAssociation(roleType);

                foreach (DynamicObject association in changedRoleByAssociation.Keys.ToArray())
                {
                    object role = changedRoleByAssociation[association];
                    roleByAssociation.TryGetValue(association, out object originalRole);

                    bool areEqual = ReferenceEquals(originalRole, role) ||
                                   (roleType.IsOne && Equals(originalRole, role)) ||
                                   (roleType.IsMany && ((IStructuralEquatable)originalRole)?.Equals((IStructuralEquatable)role) == true);

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

            foreach (DynamicAssociationType associationType in this.changedAssociationByRoleByAssociationType.Keys.ToArray())
            {
                Dictionary<DynamicObject, object> changedAssociationByRole = this.changedAssociationByRoleByAssociationType[associationType];
                Dictionary<DynamicObject, object> associationByRole = this.AssociationByRole(associationType);

                foreach (DynamicObject role in changedAssociationByRole.Keys.ToArray())
                {
                    object changedAssociation = changedAssociationByRole[role];
                    associationByRole.TryGetValue(role, out object originalRole);

                    bool areEqual = ReferenceEquals(originalRole, changedAssociation) ||
                                   (associationType.IsOne && Equals(originalRole, changedAssociation)) ||
                                   (associationType.IsMany && ((IStructuralEquatable)originalRole)?.Equals((IStructuralEquatable)changedAssociation) == true);

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

            DynamicChangeSet snapshot = new DynamicChangeSet(this.meta, this.changedRoleByAssociationByRoleType, this.changedAssociationByRoleByAssociationType);

            this.changedRoleByAssociationByRoleType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByAssociationType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            return snapshot;
        }

        internal void AddObject(DynamicObject newObject)
        {
            this.Objects = NullableArraySet.Add(this.Objects, newObject);
        }

        internal void GetRole(DynamicObject association, DynamicRoleType roleType, out object role)
        {
            if (this.changedRoleByAssociationByRoleType.TryGetValue(roleType, out Dictionary<DynamicObject, object> changedRoleByAssociation) &&
                changedRoleByAssociation.TryGetValue(association, out role))
            {
                return;
            }

            this.RoleByAssociation(roleType).TryGetValue(association, out role);
        }

        internal void SetRole(dynamic association, DynamicRoleType roleType, object role)
        {
            var normalizedRole = Normalize(roleType, role);

            if (roleType.IsUnit)
            {
                // Role
                this.ChangedRoleByAssociation(roleType)[association] = normalizedRole;
            }
            else
            {
                var associationType = roleType.AssociationType;
                this.GetRole(association, roleType, out object previousRole);
                if (roleType.IsOne)
                {
                    var roleObject = (DynamicObject)normalizedRole;
                    this.GetAssociation(roleObject, associationType, out object previousAssociation);

                    // Role
                    var changedRoleByAssociation = this.ChangedRoleByAssociation(roleType);
                    changedRoleByAssociation[association] = roleObject;

                    // Association
                    var changedAssociationByRole = this.ChangedAssociationByRole(associationType);
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
                        changedAssociationByRole[roleObject] = NullableArraySet.Remove(previousAssociation, roleObject);
                    }
                }
                else
                {
                    DynamicObject[] roles = ((IEnumerable<DynamicObject>)normalizedRole)?.ToArray() ?? Array.Empty<DynamicObject>();
                    DynamicObject[] previousRoles = (DynamicObject[])previousRole ?? Array.Empty<DynamicObject>();

                    // Use Diff (Add/Remove)
                    IEnumerable<DynamicObject> addedRoles = roles.Except(previousRoles);
                    IEnumerable<DynamicObject> removedRoles = previousRoles.Except(roles);

                    foreach (DynamicObject addedRole in addedRoles)
                    {
                        this.AddRole(association, roleType, addedRole);
                    }

                    foreach (DynamicObject removeRole in removedRoles)
                    {
                        this.RemoveRole(association, roleType, removeRole);
                    }
                }
            }
        }

        internal void AddRole(DynamicObject association, DynamicRoleType roleType, DynamicObject role)
        {
            DynamicAssociationType associationType = roleType.AssociationType;
            this.GetAssociation(role, associationType, out object previousAssociation);

            // Role
            var changedRoleByAssociation = this.ChangedRoleByAssociation(roleType);
            this.GetRole(association, roleType, out object previousRole);
            DynamicObject[] roleArray = (DynamicObject[])previousRole;
            roleArray = NullableArraySet.Add(roleArray, role);
            changedRoleByAssociation[association] = roleArray;

            // Association
            var changedAssociationByRole = this.ChangedAssociationByRole(associationType);
            if (associationType.IsOne)
            {
                // One to Many
                DynamicObject previousAssociationObject = (DynamicObject)previousAssociation;
                if (previousAssociationObject != null)
                {
                    this.GetRole(previousAssociationObject, roleType, out object previousAssociationRole);
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

        internal void RemoveRole(DynamicObject association, DynamicRoleType roleType, DynamicObject role)
        {
            DynamicAssociationType associationType = roleType.AssociationType;
            this.GetAssociation(role, associationType, out object previousAssociation);

            this.GetRole(association, roleType, out object previousRole);
            if (previousRole != null)
            {
                // Role
                var changedRoleByAssociation = this.ChangedRoleByAssociation(roleType);
                changedRoleByAssociation[association] = NullableArraySet.Remove(previousRole, role);

                // Association
                var changedAssociationByRole = this.ChangedAssociationByRole(associationType);
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

        internal void GetAssociation(DynamicObject role, DynamicAssociationType associationType, out object association)
        {
            if (this.changedAssociationByRoleByAssociationType.TryGetValue(associationType, out var changedAssociationByRole) &&
                changedAssociationByRole.TryGetValue(role, out association))
            {
                return;
            }

            this.AssociationByRole(associationType).TryGetValue(role, out association);
        }

        private Dictionary<DynamicObject, object> AssociationByRole(DynamicAssociationType asscociationType)
        {
            if (!this.associationByRoleByAssociationType.TryGetValue(asscociationType, out Dictionary<DynamicObject, object> associationByRole))
            {
                associationByRole = new Dictionary<DynamicObject, object>();
                this.associationByRoleByAssociationType[asscociationType] = associationByRole;
            }

            return associationByRole;
        }

        private Dictionary<DynamicObject, object> RoleByAssociation(DynamicRoleType roleType)
        {
            if (!this.roleByAssociationByRoleType.TryGetValue(roleType, out Dictionary<DynamicObject, object> roleByAssociation))
            {
                roleByAssociation = new Dictionary<DynamicObject, object>();
                this.roleByAssociationByRoleType[roleType] = roleByAssociation;
            }

            return roleByAssociation;
        }

        private Dictionary<DynamicObject, object> ChangedAssociationByRole(DynamicAssociationType associationType)
        {
            if (!this.changedAssociationByRoleByAssociationType.TryGetValue(associationType, out Dictionary<DynamicObject, object> changedAssociationByRole))
            {
                changedAssociationByRole = new Dictionary<DynamicObject, object>();
                this.changedAssociationByRoleByAssociationType[associationType] = changedAssociationByRole;
            }

            return changedAssociationByRole;
        }

        private Dictionary<DynamicObject, object> ChangedRoleByAssociation(DynamicRoleType roleType)
        {
            if (!this.changedRoleByAssociationByRoleType.TryGetValue(roleType, out Dictionary<DynamicObject, object> changedRoleByAssociation))
            {
                changedRoleByAssociation = new Dictionary<DynamicObject, object>();
                this.changedRoleByAssociationByRoleType[roleType] = changedRoleByAssociation;
            }

            return changedRoleByAssociation;
        }

        private object Normalize(DynamicRoleType roleType, object role)
        {
            if (role == null)
            {
                return role;
            }

            if (roleType.IsUnit)
            {
                if (role is string || role.GetType().IsValueType)
                {
                    if (role is DateTime dateTime)
                    {
                        switch (dateTime.Kind)
                        {
                            case DateTimeKind.Local:
                                dateTime = dateTime.ToUniversalTime();
                                break;
                            case DateTimeKind.Unspecified:
                                throw new ArgumentException("DateTime value is of DateTimeKind.Kind Unspecified. \nUnspecified is only allowed for DateTime.MaxValue and DateTime.MinValue, use DateTimeKind.Utc or DateTimeKind.Local instead.");
                        }

                        return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Utc);
                    }

                    return role;
                }

                throw new ArgumentException($"{role.GetType()} is not a Value Type");
            }
            else
            {
                if (roleType.IsOne)
                {
                    switch (role)
                    {
                        case System.Dynamic.DynamicObject dynamicObject:
                            return dynamicObject;

                        case IDynamic reference:
                            return reference.Instance;

                        default:
                            throw new ArgumentException($"{role.GetType()} is not a Dynamic Type");
                    }
                }
                else
                {
                    if (role is ICollection collection)
                    {
                        return this.Normalize(collection).ToArray();
                    }

                    throw new ArgumentException($"{role.GetType()} is not a collection Type");
                }
            }
        }

        private IEnumerable<dynamic> Normalize(ICollection role)
        {
            foreach (var @object in role)
            {
                switch (@object)
                {
                    case null:
                        break;

                    case System.Dynamic.DynamicObject dynamicObject:
                        yield return dynamicObject;
                        break;

                    case IDynamic reference:
                        yield return reference.Instance;
                        break;

                    default:
                        throw new ArgumentException($"{@object.GetType()} is not a Dynamic Type");
                }
            }
        }
    }
}