using Allors.Dynamic.Meta;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Allors.Dynamic
{
    public class DynamicPopulation
    {
        public DynamicMeta Meta { get; }

        private readonly HashSet<DynamicObject> objects;

        private readonly Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> roleByAssociationByType;
        private readonly Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> associationByRoleByType;

        private Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>> changedRoleByAssociationByType;
        private Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>> changedAssociationByRoleByType;

        public Dictionary<string, IDynamicDerivation> DerivationById { get; }

        public DynamicPopulation(Action<DynamicMeta> builder = null)
        {
            this.Meta = new DynamicMeta();

            this.DerivationById = new Dictionary<string, IDynamicDerivation>();

            this.objects = new HashSet<DynamicObject>();

            this.roleByAssociationByType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.associationByRoleByType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            this.changedRoleByAssociationByType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            builder?.Invoke(this.Meta);
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

        public DynamicObject NewObject()
        {
            var newObject = new DynamicObject(this);
            this.objects.Add(newObject);
            return newObject;
        }

        public DynamicChangeSet Snapshot()
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

            var snapshot = new DynamicChangeSet(this.Meta, this.changedRoleByAssociationByType, this.changedAssociationByRoleByType);

            this.changedRoleByAssociationByType = new Dictionary<DynamicRoleType, Dictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByType = new Dictionary<DynamicAssociationType, Dictionary<DynamicObject, object>>();

            return snapshot;
        }

        internal bool TryGetIndex(DynamicObject obj, GetIndexBinder binder, object[] indexes, out object result)
        {
            var name = indexes[0] as string;
            if (name != null)
            {
                this.Get(obj, name, out result);
            }
            else
            {
                result = null;
            }

            return true;
        }

        internal bool TrySetIndex(DynamicObject obj, SetIndexBinder binder, object[] indexes, object value)
        {
            var name = indexes[0] as string;
            if (name != null)
            {
                this.Set(obj, name, value);
            }

            return true;
        }

        internal bool TryGetMember(DynamicObject obj, GetMemberBinder binder, out object result)
        {
            string name = binder.Name;
            this.Get(obj, name, out result);
            return true;
        }

        internal bool TrySetMember(dynamic obj, SetMemberBinder binder, object value)
        {
            this.Set(obj, binder.Name, value);
            return true;
        }

        internal bool TryInvokeMember(dynamic obj, InvokeMemberBinder binder, object[] args, out object result)
        {
            var name = binder.Name;

            if (name.StartsWith("Add"))
            {
                var roleName = name.Substring(3);
                this.AddRole(obj, roleName, (DynamicObject)args[0]);

                result = null;
                return true;
            }

            if (name.StartsWith("Remove"))
            {
                var roleName = name.Substring(6);
                this.RemoveRole(obj, roleName, (DynamicObject)args[0]);

                result = null;
                return true;
            }

            result = null;
            return false;
        }

        internal void AddRole(DynamicObject obj, string roleName, DynamicObject objectToAdd)
        {
            this.Get(obj, roleName, out var role);

            if (role == null)
            {
                this.Set(obj, roleName, new DynamicObject[] { objectToAdd });
            }
            else
            {
                var array = role as DynamicObject[];
                if (!array.Contains(objectToAdd))
                {
                    Array.Resize(ref array, array.Length + 1);
                    array[array.Length - 1] = objectToAdd;
                    this.Set(obj, roleName, array);
                }
            }
        }

        internal void RemoveRole(DynamicObject obj, string roleName, DynamicObject objectToRemove)
        {
            this.Get(obj, roleName, out var role);

            if (role != null)
            {
                var array = role as DynamicObject[];
                var index = Array.IndexOf(array, objectToRemove);
                if (index > -1)
                {
                    array[index] = array[array.Length - 1];
                    Array.Resize(ref array, array.Length - 1);
                    this.Set(obj, roleName, array);
                }
            }
        }

        private void Get(DynamicObject obj, string name, out object result)
        {
            this.Meta.RoleTypeByName.TryGetValue(name, out var roleType);

            // Role
            if (roleType != null)
            {
                if (this.changedRoleByAssociationByType.TryGetValue(roleType, out var changeRoleByAssociation) &&
                    changeRoleByAssociation.TryGetValue(obj, out result))
                {
                    return;
                }

                var roleByAssociation = GetRoleByAssociation(roleType);
                roleByAssociation.TryGetValue(obj, out result);
            }
            // Association
            else
            {
                if (!this.Meta.AssociationTypeByName.TryGetValue(name, out var associationType))
                {
                    throw new Exception($"Unknown property {name}");
                }

                if (this.changedAssociationByRoleByType.TryGetValue(associationType, out var changedAssociationByRole) &&
                    changedAssociationByRole.TryGetValue(obj, out result))
                {
                    return;
                }

                var associationByRole = GetAssociationByRole(associationType);
                associationByRole.TryGetValue(obj, out result);
            }
        }

        private void Set(dynamic obj, string name, object value)
        {
            if (!this.Meta.RoleTypeByName.TryGetValue(name, out var roleType))
            {
                throw new Exception($"Unknown property {name}");
            }

            // Association -> Role
            {
                var changedRoleByAssociation = GetChangedRoleByAssociation(roleType);
                changedRoleByAssociation[obj] = value;
            }

            // Role -> Association
            if (value is DynamicObject role)
            {
                if (roleType.IsUnit)
                {
                    throw new Exception($"{roleType.Name} requires a Unit");
                }

                var associationType = roleType.AssociationType;

                var changedAssociationByRole = GetChangedAssociationByRole(associationType);
                changedAssociationByRole[role] = obj;
            }
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
