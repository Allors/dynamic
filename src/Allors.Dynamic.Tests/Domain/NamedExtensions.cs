namespace Allors.Dynamic.Tests.Domain
{
    public static class NamedExtensions
    {
        public static Organisation By(this Named @this)
        {
            return @this.GetAssociation<Organisation>(nameof(By));
        }
    }
}
