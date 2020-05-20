using System;
using System.Collections.Concurrent;
using System.Dynamic;
using System.Globalization;

namespace Allors.Dynamic
{
    public class Population
    {
        private readonly Inflector.Inflector inflector;

        private readonly ConcurrentDictionary<string, Derivation> derivationById;

        private readonly ConcurrentBag<AllorsDynamicObject> objects;

        private readonly ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>> roleByAssociationByRelation;
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<object, object>> associationByRoleByRelation;

        private ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>> changedRoleByAssociationByRelation;
        private ConcurrentDictionary<string, ConcurrentDictionary<object, object>> changedAssociationByRoleByRelation;

        public ConcurrentDictionary<string, Derivation> DerivationById => derivationById;

        public Population()
        {
            this.inflector = new Inflector.Inflector(new CultureInfo("en"));

            this.derivationById = new ConcurrentDictionary<string, Derivation>();

            this.objects = new ConcurrentBag<AllorsDynamicObject>();

            this.roleByAssociationByRelation = new ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>>();
            this.associationByRoleByRelation = new ConcurrentDictionary<string, ConcurrentDictionary<object, object>>();

            this.changedRoleByAssociationByRelation = new ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>>();
            this.changedAssociationByRoleByRelation = new ConcurrentDictionary<string, ConcurrentDictionary<object, object>>();
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

        public AllorsDynamicObject NewObject()
        {
            var newObject = new AllorsDynamicObject(this);
            this.objects.Add(newObject);
            return newObject;
        }

        public ChangeSet Snapshot()
        {
            var snapshot = new ChangeSet(this.changedRoleByAssociationByRelation, this.changedAssociationByRoleByRelation);

            foreach (var kvp in this.changedRoleByAssociationByRelation)
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

            foreach (var kvp in this.changedAssociationByRoleByRelation)
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

            this.changedRoleByAssociationByRelation = new ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>>();
            this.changedAssociationByRoleByRelation = new ConcurrentDictionary<string, ConcurrentDictionary<object, object>>();

            return snapshot;
        }

        internal bool TryGetIndex(AllorsDynamicObject obj, GetIndexBinder binder, object[] indexes, out object result)
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

        internal bool TrySetIndex(AllorsDynamicObject obj, SetIndexBinder binder, object[] indexes, object value)
        {
            var name = indexes[0] as string;
            if (name != null)
            {
                this.Set(obj, name, value);
            }

            return true;
        }

        internal bool TryGetMember(AllorsDynamicObject obj, GetMemberBinder binder, out object result)
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

        private void Get(AllorsDynamicObject obj, string name, out object result)
        {
            var isAssociation = name.StartsWith("Where");
            var relationName = isAssociation ? name.Substring(5) : name;

            // Role
            if (!isAssociation)
            {
                if (this.changedRoleByAssociationByRelation.TryGetValue(relationName, out var changeRoleByAssociation))
                {
                    if (changeRoleByAssociation.TryGetValue(obj, out result))
                    {
                        return;
                    }
                }

                var roleByAssociation = GetRoleByAssociation(relationName);
                roleByAssociation.TryGetValue(obj, out result);
            }
            // Association
            else
            {
                if (this.changedAssociationByRoleByRelation.TryGetValue(relationName, out var changedAssociationByRole))
                {
                    if (changedAssociationByRole.TryGetValue(obj, out result))
                    {
                        return;
                    }
                }

                var associationByRole = GetAssociationByRole(relationName);
                associationByRole.TryGetValue(obj, out result);
            }
        }

        private void Set(dynamic obj, string name, object value)
        {
            var relationName = name;

            // Association -> Role
            {
                var changedRoleByAssociation = GetChangedRoleByAssociation(relationName);

                var originalRoleByAssociation = GetRoleByAssociation(relationName);
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
            if (value is AllorsDynamicObject)
            {
                var changedAssociationByRole = GetChangedAssociationByRole(relationName);

                var originalAssociationByRole = GetAssociationByRole(relationName);
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

        private ConcurrentDictionary<object, object> GetAssociationByRole(string relationName)
        {
            // Lazy create associationByRole
            if (!this.associationByRoleByRelation.TryGetValue(relationName, out var associationByRole))
            {
                associationByRole = new ConcurrentDictionary<object, object>();
                this.associationByRoleByRelation[relationName] = associationByRole;
            }

            return associationByRole;
        }

        private ConcurrentDictionary<AllorsDynamicObject, object> GetRoleByAssociation(string relationName)
        {
            // Lazy create roleByAssociation
            if (!this.roleByAssociationByRelation.TryGetValue(relationName, out var roleByAssociation))
            {
                roleByAssociation = new ConcurrentDictionary<AllorsDynamicObject, object>();
                this.roleByAssociationByRelation[relationName] = roleByAssociation;
            }

            return roleByAssociation;
        }

        private ConcurrentDictionary<object, object> GetChangedAssociationByRole(string relationName)
        {
            // Lazy create associationByRole
            if (!this.changedAssociationByRoleByRelation.TryGetValue(relationName, out var changedAssociationByRole))
            {
                changedAssociationByRole = new ConcurrentDictionary<object, object>();
                this.changedAssociationByRoleByRelation[relationName] = changedAssociationByRole;
            }

            return changedAssociationByRole;
        }

        private ConcurrentDictionary<AllorsDynamicObject, object> GetChangedRoleByAssociation(string relationName)
        {
            // Lazy create roleByAssociation
            if (!this.changedRoleByAssociationByRelation.TryGetValue(relationName, out var changedRoleByAssociation))
            {
                changedRoleByAssociation = new ConcurrentDictionary<AllorsDynamicObject, object>();
                this.changedRoleByAssociationByRelation[relationName] = changedRoleByAssociation;
            }

            return changedRoleByAssociation;
        }
    }
}
