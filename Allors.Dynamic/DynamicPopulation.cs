using Allors.Dynamic.Meta;
using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Runtime.InteropServices.ComTypes;

namespace Allors.Dynamic
{
    public class DynamicPopulation
    {
        public DynamicMeta Meta { get; }

        private readonly ConcurrentBag<DynamicObject> objects;

        private readonly ConcurrentDictionary<DynamicRoleType, ConcurrentDictionary<DynamicObject, object>> roleByAssociationByType;
        private readonly ConcurrentDictionary<DynamicAssociationType, ConcurrentDictionary<object, object>> associationByRoleByType;

        private ConcurrentDictionary<DynamicRoleType, ConcurrentDictionary<DynamicObject, object>> changedRoleByAssociationByType;
        private ConcurrentDictionary<DynamicAssociationType, ConcurrentDictionary<object, object>> changedAssociationByRoleByType;

        public ConcurrentDictionary<string, IDynamicDerivation> DerivationById { get; }

        public DynamicPopulation(Action<DynamicMeta> builder = null)
        {
            this.Meta = new DynamicMeta();

            this.DerivationById = new ConcurrentDictionary<string, IDynamicDerivation>();

            this.objects = new ConcurrentBag<DynamicObject>();

            this.roleByAssociationByType = new ConcurrentDictionary<DynamicRoleType, ConcurrentDictionary<DynamicObject, object>>();
            this.associationByRoleByType = new ConcurrentDictionary<DynamicAssociationType, ConcurrentDictionary<object, object>>();

            this.changedRoleByAssociationByType = new ConcurrentDictionary<DynamicRoleType, ConcurrentDictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByType = new ConcurrentDictionary<DynamicAssociationType, ConcurrentDictionary<object, object>>();

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
            var snapshot = new DynamicChangeSet(this.Meta, this.changedRoleByAssociationByType, this.changedAssociationByRoleByType);

            foreach (var kvp in this.changedRoleByAssociationByType)
            {
                var relation = kvp.Key;
                var changedRoleByAssociation = kvp.Value;

                var roleByAssociation = GetRoleByAssociation(relation);

                foreach (var kvp2 in changedRoleByAssociation)
                {
                    var association = kvp2.Key;
                    var changedRole = kvp2.Value;

                    roleByAssociation[association] = changedRole;
                }
            }

            foreach (var kvp in this.changedAssociationByRoleByType)
            {
                var relation = kvp.Key;
                var changedRoleByAssociation = kvp.Value;

                var associationByRole = GetAssociationByRole(relation);

                foreach (var kvp2 in changedRoleByAssociation)
                {
                    var role = kvp2.Key;
                    var changedAssociation = kvp2.Value;

                    associationByRole[role] = changedAssociation;
                }
            }

            this.changedRoleByAssociationByType = new ConcurrentDictionary<DynamicRoleType, ConcurrentDictionary<DynamicObject, object>>();
            this.changedAssociationByRoleByType = new ConcurrentDictionary<DynamicAssociationType, ConcurrentDictionary<object, object>>();

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
            //var name = binder.Name;
            //result = null;

            //if (name.StartsWith("Add"))
            //{
            //    this.population.AddRole(binder, args);
            //    return true;
            //}

            //if (name.StartsWith("Remove"))
            //{
            //    var roleName = name.Substring(6);
            //    roleName = this.inflector.Pluralize(roleName);
            //    this.population.RemoveRole(roleName);

            //    return true;
            //}

            result = null;
            return false;
        }

        //internal void AddRole(InvokeMemberBinder binder, object[] args)
        //{
        //    var newRole = args[0];

        //    var roleName = name.Substring(3);
        //    roleName = this.inflector.Pluralize(roleName);

        //    if (this.relations.TryGetValue(roleName, out var roles))
        //    {
        //        var newRoles = new List<dynamic>((IEnumerable<object>)roles);
        //        newRoles.Add(newRole);
        //        this.relations[roleName] = newRoles;
        //    }
        //    else
        //    {
        //        this.relations[roleName] = new dynamic[] { newRole };
        //    }

        //}

        //internal void RemoveRole(InvokeMemberBinder binder)
        //{
        //    this.population.RemoveRole(binder, args);

        //    var newRole = args[0];
        //    if (this.relations.TryGetValue(roleName, out var roles))
        //    {
        //        var newRoles = new List<dynamic>((dynamic[])roles);
        //        newRoles.Remove(newRole);
        //        this.relations[roleName] = newRoles.ToArray();
        //    }

        //}

        private void Get(DynamicObject obj, string name, out object result)
        {
            this.Meta.RoleTypeByName.TryGetValue(name, out var roleType);

            // Role
            if (roleType != null)
            {
                if (this.changedRoleByAssociationByType.TryGetValue(roleType, out var changeRoleByAssociation))
                {
                    if (changeRoleByAssociation.TryGetValue(obj, out result))
                    {
                        return;
                    }
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

                if (this.changedAssociationByRoleByType.TryGetValue(associationType, out var changedAssociationByRole))
                {
                    if (changedAssociationByRole.TryGetValue(obj, out result))
                    {
                        return;
                    }
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

                var originalRoleByAssociation = GetRoleByAssociation(roleType);
                originalRoleByAssociation.TryGetValue(obj, out object originalRole);
                if (Equals(originalRole, value))
                {
                    changedRoleByAssociation.TryRemove(obj, out object deletedValue);
                }
                else
                {
                    changedRoleByAssociation[obj] = value;
                }
            }

            // Role -> Association
            if (value is DynamicObject)
            {
                if (roleType.IsUnit)
                {
                    throw new Exception($"{roleType.Name} requires a Unit");
                }

                var associationType = roleType.AssociationType;

                var changedAssociationByRole = GetChangedAssociationByRole(associationType);

                var originalAssociationByRole = GetAssociationByRole(associationType);
                originalAssociationByRole.TryGetValue(value, out object originalAssociation);
                if (Equals(originalAssociation, obj))
                {
                    changedAssociationByRole.TryRemove(value, out object deletedValue);
                }
                else
                {
                    changedAssociationByRole[value] = obj;
                }
            }
        }

        private ConcurrentDictionary<object, object> GetAssociationByRole(DynamicAssociationType associationType)
        {
            // Lazy create associationByRole
            if (!this.associationByRoleByType.TryGetValue(associationType, out var associationByRole))
            {
                associationByRole = new ConcurrentDictionary<object, object>();
                this.associationByRoleByType[associationType] = associationByRole;
            }

            return associationByRole;
        }

        private ConcurrentDictionary<DynamicObject, object> GetRoleByAssociation(DynamicRoleType roleType)
        {
            // Lazy create roleByAssociation
            if (!this.roleByAssociationByType.TryGetValue(roleType, out var roleByAssociation))
            {
                roleByAssociation = new ConcurrentDictionary<DynamicObject, object>();
                this.roleByAssociationByType[roleType] = roleByAssociation;
            }

            return roleByAssociation;
        }

        private ConcurrentDictionary<object, object> GetChangedAssociationByRole(DynamicAssociationType associationType)
        {
            // Lazy create associationByRole
            if (!this.changedAssociationByRoleByType.TryGetValue(associationType, out var changedAssociationByRole))
            {
                changedAssociationByRole = new ConcurrentDictionary<object, object>();
                this.changedAssociationByRoleByType[associationType] = changedAssociationByRole;
            }

            return changedAssociationByRole;
        }

        private ConcurrentDictionary<DynamicObject, object> GetChangedRoleByAssociation(DynamicRoleType roleType)
        {
            // Lazy create roleByAssociation
            if (!this.changedRoleByAssociationByType.TryGetValue(roleType, out var changedRoleByAssociation))
            {
                changedRoleByAssociation = new ConcurrentDictionary<DynamicObject, object>();
                this.changedRoleByAssociationByType[roleType] = changedRoleByAssociation;
            }

            return changedRoleByAssociation;
        }
    }
}
