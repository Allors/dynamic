namespace Allors.Dynamic.Tests
{
    using System.Globalization;
    using Inflector;

    public class Pluralizer : IPluralizer
    {
        private readonly Inflector inflector;

        public Pluralizer()
        {
            this.inflector = new global::Inflector.Inflector(new CultureInfo("en"));
        }

        public string Pluralize(string singular) => this.inflector.Pluralize(singular);
    }
}
