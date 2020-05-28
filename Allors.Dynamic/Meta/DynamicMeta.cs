namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;

    public class DynamicMeta
    {
        private readonly Inflector.Inflector inflector;

        internal DynamicMeta()
        {
            this.LinkerTypeByName = new Dictionary<string, DynamicLinkerType>();
            this.LinkedTypeByName = new Dictionary<string, DynamicLinkedType>();

            this.inflector = new Inflector.Inflector(new CultureInfo("en"));
        }

        public Dictionary<string, DynamicLinkerType> LinkerTypeByName { get; }

        public Dictionary<string, DynamicLinkedType> LinkedTypeByName { get; }

        public DynamicMeta AddDataAssociation(string name)
        {
            var linkedAssociationEnd = new DynamicLinkedType
            {
                SingularName = name,
                PluralName = inflector.Pluralize(name),
                IsMany = false,
            };

            this.AddLinkedType(linkedAssociationEnd);

            return this;
        }

        public DynamicMeta AddOneToOneAssociation(string linkingName, string linkedName)
        {
            return this.AddAssociation(linkingName, false, linkedName, false);
        }

        public DynamicMeta AddOneToManyAssociation(string linkingName, string linkedName)
        {
            return this.AddAssociation(linkingName, false, linkedName, true);
        }

        public DynamicMeta AddManyToOneAssociation(string linkingName, string linkedName)
        {
            return this.AddAssociation(linkingName, true, linkedName, false);
        }

        public DynamicMeta AddManyToManyAssociation(string linkingName, string linkedName)
        {
            return this.AddAssociation(linkingName, true, linkedName, true);
        }

        private DynamicMeta AddAssociation(string linkerName, bool linkerIsMany, string linkedName, bool linkedIsMany)
        {
            var linkedAssociationEnd = new DynamicLinkedType
            {
                SingularName = linkedName,
                PluralName = this.inflector.Pluralize(linkedName),
                IsMany = linkedIsMany,
            };

            this.AddLinkedType(linkedAssociationEnd);

            var linkingAssociationEnd = new DynamicLinkerType(linkedAssociationEnd)
            {
                SingularName = linkerName,
                PluralName = this.inflector.Pluralize(linkerName),
                IsMany = linkerIsMany,
            };

            this.AddLinkerType(linkingAssociationEnd);

            return this;
        }

        private void AddLinkerType(DynamicLinkerType linkerType)
        {
            this.CheckNames(linkerType.SingularName, linkerType.PluralName);

            this.LinkerTypeByName.Add(linkerType.SingularName, linkerType);
            this.LinkerTypeByName.Add(linkerType.PluralName, linkerType);
        }

        private void AddLinkedType(DynamicLinkedType linkedType)
        {
            this.CheckNames(linkedType.SingularName, linkedType.PluralName);

            this.LinkedTypeByName.Add(linkedType.SingularName, linkedType);
            this.LinkedTypeByName.Add(linkedType.PluralName, linkedType);
        }

        private void CheckNames(string singularName, string pluralName)
        {
            if (this.LinkedTypeByName.ContainsKey(singularName) ||
                this.LinkerTypeByName.ContainsKey(singularName))
            {
                throw new Exception($"{singularName} is not unique");
            }

            if (this.LinkedTypeByName.ContainsKey(pluralName) ||
                this.LinkerTypeByName.ContainsKey(pluralName))
            {
                throw new Exception($"{pluralName} is not unique");
            }
        }
    }
}