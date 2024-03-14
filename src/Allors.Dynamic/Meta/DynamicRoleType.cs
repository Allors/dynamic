using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Allors.Dynamic.Meta
{
    public class DynamicRoleType
    {
        public DynamicAssociationType AssociationType { get; internal set; } = null!;

        public DynamicObjectType ObjectType { get; }

        public string SingularName { get; }

        public string PluralName { get; }

        public string Name { get; }

        public bool IsOne { get; }

        public bool IsMany { get;}

        public bool IsUnit { get;  }

        public void Deconstruct(out DynamicAssociationType associationType, out DynamicRoleType roleType)
        {
            associationType = this.AssociationType;
            roleType = this;
        }

        internal DynamicRoleType(DynamicObjectType objectType, string singularName, string pluralName, string name, bool isOne, bool isMany, bool isUnit)
        {
            this.ObjectType = objectType;
            this.SingularName = singularName;
            this.PluralName = pluralName;
            this.Name = name;
            this.IsOne = isOne;
            this.IsMany = isMany;
            this.IsUnit = isUnit;
        }

        public override string ToString()
        {
            return this.Name;
        }

        internal string SingularNameForEmbeddedAssociationType(DynamicObjectType dynamicObjectType)
        {
            return $"{dynamicObjectType.Type.Name}Where{this.SingularName}";
        }

        internal string PluralNameForEmbeddedAssociationType(DynamicObjectType dynamicObjectType)
        {
            return $"{this.ObjectType.DynamicMeta.Pluralize(dynamicObjectType.Type.Name)}Where{this.SingularName}";
        }

        internal object? Normalize(object? value)
        {
            if (this.IsUnit)
            {
                return this.NormalizeUnit(value);
            }

            return this.IsOne switch
            {
                true => this.NormalizeToOne(value),
                _ => this.NormalizeToMany(value)
            };
        }

        private object NormalizeUnit(object value)
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

            if (value.GetType() != this.ObjectType.Type)
            {
                value = Convert.ChangeType(value, this.ObjectType.TypeCode);
            }

            return value;
        }

        private object? NormalizeToOne(object? value)
        {
            if (value != null)
            {
                var type = this.ObjectType.Type;
                if (!type.IsInstanceOfType(value))
                {
                    throw new ArgumentException($"{this.Name} should be a {type.Name} but was a {value.GetType()}");
                }
            }

            return value;
        }

        private object? NormalizeToMany(object? value)
        {
            return value switch
            {
                null => null,
                ICollection collection => this.NormalizeToMany(collection).ToArray(),
                _ => throw new ArgumentException($"{value.GetType()} is not a collection Type")
            };
        }

        private IEnumerable<IDynamicObject> NormalizeToMany(ICollection role)
        {
            foreach (IDynamicObject @object in role)
            {
                if (@object != null)
                {
                    var type = this.ObjectType.Type;

                    if (!type.IsInstanceOfType(@object))
                    {
                        throw new ArgumentException($"{this.Name} should be a {type.Name} but was a {@object.GetType()}");
                    }

                    yield return @object;
                }
            }
        }
    }
}
