using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Binding
{
    public sealed class DynamicObject : System.Dynamic.DynamicObject, IDynamicObject
    {
        internal DynamicObject(DynamicPopulation population, DynamicObjectType objectType)
        {
            Population = population;
            ObjectType = objectType;
        }

        IDynamicPopulation IDynamicObject.Population => this.Population;

        public DynamicPopulation Population { get; }


        public DynamicObjectType ObjectType { get; }

        public object GetRole(string name) => Population.GetRole(this, ObjectType.RoleTypeByName[name]);

        public void SetRole(string name, object value) => Population.SetRole(this, ObjectType.RoleTypeByName[name], value);

        public void AddRole(string name, IDynamicObject value) => Population.AddRole(this, (IDynamicManyRoleType)ObjectType.RoleTypeByName[name], value);

        public void RemoveRole(string name, IDynamicObject value) => Population.RemoveRole(this, (IDynamicManyRoleType)ObjectType.RoleTypeByName[name], value);

        public object GetAssociation(string name) => Population.GetAssociation(this, (IDynamicCompositeAssociationType)ObjectType.AssociationTypeByName[name]);

        /// <inheritdoc/>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return TryGet(indexes[0], out result);
        }

        /// <inheritdoc/>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return TrySet(indexes[0], value);
        }

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGet(binder.Name, out result);
        }

        /// <inheritdoc/>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return TrySet(binder.Name, value);
        }

        /// <inheritdoc/>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var name = binder.Name;

            result = null;

            if (name.StartsWith("Add") && ObjectType.RoleTypeByName.TryGetValue(name.Substring(3), out var roleType))
            {
                Population.AddRole(this, (IDynamicCompositeRoleType)roleType, (DynamicObject)args[0]);
                return true;
            }

            if (name.StartsWith("Remove") && ObjectType.RoleTypeByName.TryGetValue(name.Substring(6), out roleType))
            {
                // TODO: RemoveAll
                Population.RemoveRole(this, (IDynamicCompositeRoleType)roleType, (DynamicObject)args[0]);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var roleType in this.ObjectType.RoleTypeByName.Values.ToArray().Distinct())
            {
                yield return roleType.Name;
            }

            foreach (var associationType in this.ObjectType.AssociationTypeByName.Values.ToArray().Distinct())
            {
                yield return associationType.Name;
            }
        }

        private bool TryGet(object nameOrType, out object result)
        {
            switch (nameOrType)
            {
                case string name:
                    {
                        if (ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                        {
                            return TryGetRole(roleType, out result);
                        }

                        if (ObjectType.AssociationTypeByName.TryGetValue(name, out var associationType))
                        {
                            return TryGetAssociation((IDynamicCompositeAssociationType)associationType, out result);
                        }
                    }

                    break;

                case IDynamicRoleType roleType:
                    return TryGetRole(roleType, out result);

                case IDynamicAssociationType associationType:
                    return TryGetAssociation((IDynamicCompositeAssociationType)associationType, out result);
            }

            result = null;
            return false;
        }

        private bool TryGetRole(IDynamicRoleType roleType, out object result)
        {
            result = Population.GetRole(this, roleType);
            if (result == null && roleType.IsMany)
            {
                result = Array.Empty<DynamicObject>();
            }

            return true;
        }

        private bool TryGetAssociation(IDynamicCompositeAssociationType associationType, out object result)
        {
            result = Population.GetAssociation(this, associationType);
            if (result == null && associationType.IsMany)
            {
                result = Array.Empty<DynamicObject>();
            }

            return true;
        }

        private bool TrySet(object nameOrType, object value)
        {
            switch (nameOrType)
            {
                case string name:
                    {
                        if (ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                        {
                            Population.SetRole(this, roleType, value);
                            return true;
                        }
                    }

                    break;

                case IDynamicRoleType roleType:
                    Population.SetRole(this, roleType, value);
                    return true;
            }

            return false;
        }
    }
}