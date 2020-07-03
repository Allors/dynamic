namespace Allors.Dynamic.Tests.Domain
{
    public static class PersonExtensions
    {
        public static string Name(this Person @this)
        {
            return @this.GetRole<string>(nameof(Name));
        }

        public static Person Name(this Person @this, string value)
        {
            @this.SetRole(nameof(Name), value);
            return @this;
        }

        public static Organisation OrganisationWhereOwner(this Person @this)
        {
            return @this.GetAssociation<Organisation>(nameof(OrganisationWhereOwner));
        }
    }
}
