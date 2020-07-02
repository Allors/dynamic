namespace Allors.Dynamic
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using Allors.Dynamic.Meta;

    public abstract class DynamicObject : System.Dynamic.DynamicObject, IDynamicObject
    {
        private readonly DynamicPopulation population;

        protected DynamicObject(DynamicPopulation population)
        {
            this.population = population;
        }

        /// <inheritdoc/>
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return this.population.TryGetIndex(this, binder, indexes, out result);
        }

        /// <inheritdoc/>
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return this.population.TrySetIndex(this, binder, indexes, value);
        }

        /// <inheritdoc/>
        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.population.TryGetMember(this, binder, out result);
        }

        /// <inheritdoc/>
        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return this.population.TrySetMember(this, binder, value);
        }

        /// <inheritdoc/>
        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return this.population.TryInvokeMember(this, binder, args, out result);
        }

        /// <inheritdoc/>
        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (DynamicRoleType roleType in this.population.Meta.RoleTypeByName.Values.ToArray().Distinct())
            {
                yield return roleType.Name;
            }

            foreach (DynamicAssociationType associationType in this.population.Meta.AssociationTypeByName.Values.ToArray().Distinct())
            {
                yield return associationType.Name;
            }
        }

        public void Get(DynamicRoleType roleType, out object result) => this.population.Get(this, roleType, out result);

        public void Get(DynamicAssociationType associationType, out object result) => this.population.Get(this, associationType, out result);

        public void Set(DynamicRoleType roleType, object role) => this.population.Set(this, roleType, role);

        public void Add(DynamicRoleType roleType, DynamicObject role) => this.population.Add(this, roleType, role);

        public void Remove(DynamicRoleType roleType, DynamicObject role) => this.population.Remove(this, roleType, role);

        public T GetRole<T>(string name) => this.population.GetRole<T>(this, name);

        public void SetRole<T>(string name, T value) => this.population.SetRole<T>(this, name, value);

        public T GetAssociation<T>(string name) => this.population.GetAssociation<T>(this, name);
    }
}