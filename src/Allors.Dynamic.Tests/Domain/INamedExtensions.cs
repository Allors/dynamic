namespace Allors.Dynamic.Tests.Domain
{
    public static class INamedExtensions
    {
        public static Organization OrganisationWhereNamed(this INamed @this)
        {
            return (Organization)@this.GetAssociation(nameof(OrganisationWhereNamed));
        }
    }
}
