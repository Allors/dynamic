namespace Allors.Dynamic.Tests.Domain
{
    public static class INamedExtensions
    {
        public static Organization OrganizationWhereNamed(this INamed @this)
        {
            return (Organization)@this.GetAssociation(nameof(OrganizationWhereNamed));
        }
    }
}
