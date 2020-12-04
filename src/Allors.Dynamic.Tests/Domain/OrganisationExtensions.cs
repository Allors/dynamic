namespace Allors.Dynamic.Tests.Domain
{
    public static class OrganisationExtensions
    {
        public static string Name(this Organization @this) => (string)@this.GetRole(nameof(Name));

        public static Organization Name(this Organization @this, string value)
        {
            @this.SetRole(nameof(Name), value);
            return @this;
        }

        public static Person Owner(this Organization @this) => (Person)@this.GetRole(nameof(Owner));

        public static Organization Owner(this Organization @this, Person value)
        {
            @this.SetRole(nameof(Owner), value);
            return @this;
        }

        public static INamed Named(this Organization @this) => (Domain.INamed)@this.GetRole(nameof(Named));

        public static Organization Named(this Organization @this, INamed value)
        {
            @this.SetRole(nameof(Named), value);
            return @this;
        }
    }
}
