using System.Collections.Concurrent;
using System.Dynamic;
using System.Globalization;

namespace Allors.Dynamic
{
    public class Population
    {
        private readonly Inflector.Inflector inflector;

        private ConcurrentBag<AllorsDynamicObject> objects;

        private ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>> roleByAssociationByRelation;

        private ConcurrentDictionary<string, ConcurrentDictionary<object, object>> associationByRoleByRelation;

        public Population()
        {
            this.inflector = new Inflector.Inflector(new CultureInfo("en"));

            this.objects = new ConcurrentBag<AllorsDynamicObject>();

            this.roleByAssociationByRelation = new ConcurrentDictionary<string, ConcurrentDictionary<AllorsDynamicObject, object>>();
            this.associationByRoleByRelation = new ConcurrentDictionary<string, ConcurrentDictionary<object, object>>();
        }

        public AllorsDynamicObject NewObject()
        {
            var newObject = new AllorsDynamicObject(this);
            this.objects.Add(newObject);
            return newObject;
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
                // Lazy create roleByAssociation
                if (!this.roleByAssociationByRelation.TryGetValue(relationName, out var roleByAssociation))
                {
                    roleByAssociation = new ConcurrentDictionary<AllorsDynamicObject, object>();
                    this.roleByAssociationByRelation[relationName] = roleByAssociation;
                }

                roleByAssociation.TryGetValue(obj, out result);
            }
            // Association
            else
            {
                // Lazy create associationByRole
                if (!this.associationByRoleByRelation.TryGetValue(relationName, out var associationByRole))
                {
                    associationByRole = new ConcurrentDictionary<object, object>();
                    this.associationByRoleByRelation[relationName] = associationByRole;
                }

                associationByRole.TryGetValue(obj, out result);
            }
        }

        private void Set(dynamic obj, string name, object value)
        {
            var relationName = name;

            // Association -> Role
            {
                if (!this.roleByAssociationByRelation.TryGetValue(relationName, out var roleByAssociation))
                {
                    roleByAssociation = new ConcurrentDictionary<AllorsDynamicObject, object>();
                    this.roleByAssociationByRelation[relationName] = roleByAssociation;
                }

                roleByAssociation[obj] = value;
            }

            // Role -> Association
            if (value is AllorsDynamicObject)
            {
                if (!this.associationByRoleByRelation.TryGetValue(relationName, out var associationByRole))
                {
                    associationByRole = new ConcurrentDictionary<object, object>();
                    this.associationByRoleByRelation[relationName] = associationByRole;
                }

                associationByRole[value] = obj;
            }

        }
    }
}
