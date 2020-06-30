namespace Allors.Dynamic.Meta
{
    public class DynamicUnitRoleType<T> : DynamicUnitRoleType
    {
        public DynamicUnitRoleType(DynamicMeta meta) : base(meta, typeof(T))
        {
        }

        public void Deconstruct(out DynamicAssociationType associationType, out DynamicUnitRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }
    }
}