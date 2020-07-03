namespace Allors.Dynamic.Tests.Domain
{
    public static class NamedExtensions
    {
        public static Organisation OrganisationWhereNamed(this Named @this)
        {
            return @this.GetAssociation<Organisation>(nameof(OrganisationWhereNamed));
        }
    }
}
