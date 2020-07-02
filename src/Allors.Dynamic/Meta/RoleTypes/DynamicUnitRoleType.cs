namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections;

    public class DynamicUnitRoleType : DynamicRoleType
    {
        public DynamicUnitRoleType(DynamicMeta meta, Type type)
        {
            this.Meta = meta;
            this.Type = type;
        }

        public Type Type { get; }

        public TypeCode TypeCode { get; }

        public DynamicMeta Meta { get; }

        public DynamicAssociationType AssociationType { get; internal set; }

        public string Name => this.SingularName;

        public string SingularName { get; internal set; }

        public string PluralName { get; internal set; }

        public bool IsOne => true;

        public bool IsMany => false;

        public bool IsUnit => true;

        /// <inheritdoc/>
        public override string ToString()
        {
            return this.Name;
        }

        public object Normalize(object value)
        {
            if (value == null)
            {
                return value;
            }

            if (value is DateTime dateTime && dateTime != DateTime.MinValue && dateTime != DateTime.MaxValue)
            {
                switch (dateTime.Kind)
                {
                    case DateTimeKind.Local:
                        dateTime = dateTime.ToUniversalTime();
                        break;
                    case DateTimeKind.Unspecified:
                        throw new ArgumentException(
@"DateTime value is of DateTimeKind.Kind Unspecified.
Unspecified is only allowed for DateTime.MaxValue and DateTime.MinValue. 
Use DateTimeKind.Utc or DateTimeKind.Local.");
                }

                return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second, dateTime.Millisecond, DateTimeKind.Utc);
            }

            if (value.GetType() != this.Type)
            {
                value = Convert.ChangeType(value, this.TypeCode);
            }

            return value;
        }
    }
}