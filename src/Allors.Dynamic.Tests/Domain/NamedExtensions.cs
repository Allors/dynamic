namespace Allors.Dynamic.Tests.Domain
{
    public static class NamedExtensions
    {
        public static Organisation OrganisationWhereNamed(this Named @this)
        {
            return (Organisation)@this.GetAssociation(nameof(OrganisationWhereNamed));
        }
    }
}
