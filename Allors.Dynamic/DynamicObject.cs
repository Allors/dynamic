namespace Allors.Dynamic
{
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;

    public class DynamicObject : System.Dynamic.DynamicObject
    {
        private readonly DynamicPopulation population;

        internal DynamicObject(DynamicPopulation population)
        {
            this.population = population;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            return this.population.TryGetIndex(this, binder, indexes, out result);
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            return this.population.TrySetIndex(this, binder, indexes, value);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.population.TryGetMember(this, binder, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return this.population.TrySetMember(this, binder, value);
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            return this.population.TryInvokeMember(this, binder, args, out result);
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (Meta.DynamicRoleType roleType in this.population.Meta.RoleTypeByName.Values.ToArray().Distinct())
            {
                yield return roleType.Name;
            }

            foreach (Meta.DynamicAssociationType associationType in this.population.Meta.AssociationTypeByName.Values.ToArray().Distinct()
            )
            {
                yield return associationType.Name;
            }
        }
    }
}