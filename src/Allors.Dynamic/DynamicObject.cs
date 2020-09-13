namespace Allors.Dynamic
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Allors.Dynamic.Meta;

    public abstract class DynamicObject : System.Dynamic.DynamicObject, IDynamicObject
    {
        protected DynamicObject(IDynamicPopulation population, DynamicObjectType objectType)
        {
            this.Population = population;
            this.ObjectType = objectType;
        }

        public IDynamicPopulation Population { get; }

        public DynamicObjectType ObjectType { get; }

        public object GetRole(string name) => this.Population.GetRole(this, this.ObjectType.RoleTypeByName[name]);

        public void SetRole(string name, object value) => this.Population.SetRole(this, this.ObjectType.RoleTypeByName[name], value);

        public void AddRole(string name, IDynamicObject value) => this.Population.AddRole(this, this.ObjectType.RoleTypeByName[name], value);

        public void RemoveRole(string name, IDynamicObject value) => this.Population.RemoveRole(this, this.ObjectType.RoleTypeByName[name], value);

        public object GetAssociation(string name) => this.Population.GetAssociation(this, this.ObjectType.AssociationTypeByName[name]);

        /// <inheritdoc/>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return this.TryGet(indexes[0], out result);
        }

        /// <inheritdoc/>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return this.TrySet(indexes[0], value);
        }

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.TryGet(binder.Name, out result);
        }

        /// <inheritdoc/>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return this.TrySet(binder.Name, value);
        }

        /// <inheritdoc/>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var name = binder.Name;

            result = null;

            if (name.StartsWith("Add") && this.ObjectType.RoleTypeByName.TryGetValue(name.Substring(3), out var roleType))
            {
                this.Population.AddRole(this, roleType, (DynamicObject)args[0]);
                return true;
            }

            if (name.StartsWith("Remove") && this.ObjectType.RoleTypeByName.TryGetValue(name.Substring(6), out roleType))
            {
                // TODO: RemoveAll
                this.Population.RemoveRole(this, roleType, (DynamicObject)args[0]);
                return true;
            }

            return false;
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            var objectType = this.Population.Meta.ObjectTypeByType[this.GetType()];
            foreach (var roleType in objectType.RoleTypeByName.Values.ToArray().Distinct())
            {
                yield return roleType.Name;
            }

            foreach (var associationType in objectType.AssociationTypeByName.Values.ToArray().Distinct())
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
                        if (this.ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                        {
                            return this.TryGetRole(roleType, out result);
                        }

                        if (this.ObjectType.AssociationTypeByName.TryGetValue(name, out var associationType))
                        {
                            return this.TryGetAssociation(associationType, out result);
                        }
                    }

                    break;

                case IDynamicRoleType roleType:
                    return this.TryGetRole(roleType, out result);

                case IDynamicAssociationType associationType:
                    return this.TryGetAssociation(associationType, out result);
            }

            result = null;
            return false;
        }

        private bool TryGetRole(IDynamicRoleType roleType, out object result)
        {
            result = this.Population.GetRole(this, roleType);
            if (result == null && roleType.IsMany)
            {
                result = roleType.ObjectType.EmptyArray;
            }

            return true;
        }

        private bool TryGetAssociation(IDynamicAssociationType associationType, out object result)
        {
            result = this.Population.GetAssociation(this, associationType);
            if (result == null && associationType.IsMany)
            {
                result = associationType.ObjectType.EmptyArray;
            }

            return true;
        }

        private bool TrySet(object nameOrType, object value)
        {
            switch (nameOrType)
            {
                case string name:
                    {
                        if (this.ObjectType.RoleTypeByName.TryGetValue(name, out var roleType))
                        {
                            this.Population.SetRole(this, roleType, value);
                            return true;
                        }
                    }

                    break;

                case IDynamicRoleType roleType:
                    this.Population.SetRole(this, roleType, value);
                    return true;
            }

            return false;
        }
    }
}