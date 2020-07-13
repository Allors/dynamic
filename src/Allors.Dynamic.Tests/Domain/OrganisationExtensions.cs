namespace Allors.Dynamic.Tests.Domain
{
    public static class OrganisationExtensions
    {
        public static string Name(this Organisation @this) => (string)@this.GetRole(nameof(Name));

        public static Organisation Name(this Organisation @this, string value)
        {
            @this.SetRole(nameof(Name), value);
            return @this;
        }

        public static Person Owner(this Organisation @this) => (Person)@this.GetRole(nameof(Owner));

        public static Organisation Owner(this Organisation @this, Person value)
        {
            @this.SetRole(nameof(Owner), value);
            return @this;
        }

        public static Named Named(this Organisation @this) => (Domain.Named)@this.GetRole(nameof(Named));

        public static Organisation Named(this Organisation @this, Named value)
        {
            @this.SetRole(nameof(Named), value);
            return @this;
        }
    }
}
