namespace Allors.Dynamic.Meta
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DynamicObjectType
    {
        private readonly IDictionary<string, IDynamicAssociationType> assignedAssociationTypeByName;

        private readonly IDictionary<string, IDynamicRoleType> assignedRoleTypeByName;

        private IDictionary<string, IDynamicAssociationType> derivedAssociationTypeByName;

        private IDictionary<string, IDynamicRoleType> derivedRoleTypeByName;

        public DynamicMeta Meta { get; }

        public Type Type { get; }

        public TypeCode TypeCode { get; }

        public ISet<DynamicObjectType> SuperTypes { get; }

        internal object EmptyArray { get; }

        public IDictionary<string, IDynamicAssociationType> AssociationTypeByName
        {
            get
            {
                if (this.derivedAssociationTypeByName == null)
                {
                    this.derivedAssociationTypeByName = new Dictionary<string, IDynamicAssociationType>(this.assignedAssociationTypeByName);
                    foreach (KeyValuePair<string, IDynamicAssociationType> item in this.SuperTypes.SelectMany(v => v.assignedAssociationTypeByName))
                    {
                        this.derivedAssociationTypeByName[item.Key] = item.Value;
                    }
                }

                return this.derivedAssociationTypeByName;
            }
        }

        public IDictionary<string, IDynamicRoleType> RoleTypeByName
        {
            get
            {
                if (this.derivedRoleTypeByName == null)
                {
                    this.derivedRoleTypeByName = new Dictionary<string, IDynamicRoleType>(this.assignedRoleTypeByName);
                    foreach (KeyValuePair<string, IDynamicRoleType> item in this.SuperTypes.SelectMany(v => v.assignedRoleTypeByName))
                    {
                        this.derivedRoleTypeByName[item.Key] = item.Value;
                    }
                }

                return this.derivedRoleTypeByName;
            }
        }

        internal DynamicObjectType(DynamicMeta meta, Type type)
        {
            this.Meta = meta;
            this.Type = type;
            this.TypeCode = Type.GetTypeCode(type);
            this.SuperTypes = new HashSet<DynamicObjectType>();
            this.assignedAssociationTypeByName = new Dictionary<string, IDynamicAssociationType>();
            this.assignedRoleTypeByName = new Dictionary<string, IDynamicRoleType>();

            this.EmptyArray = Array.CreateInstance(type, 0);

            var hierarchyChanged = false;
            foreach (DynamicObjectType other in meta.ObjectTypeByType.Values)
            {
                if (this.Type.IsAssignableFrom(other.Type))
                {
                    other.SuperTypes.Add(this);
                    hierarchyChanged = true;
                }

                if (other.Type.IsAssignableFrom(this.Type))
                {
                    this.SuperTypes.Add(other);
                    hierarchyChanged = true;
                }
            }

            if (hierarchyChanged)
            {
                this.Meta.ResetDerivations();
            }
        }

        public DynamicUnitRoleType AddUnit(DynamicObjectType roleObjectType, string roleName)
        {
            var roleType = new DynamicUnitRoleType(roleObjectType, roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicUnitAssociationType(this, roleType);
            roleObjectType.AddAssociationType(associationType);

            this.Meta.ResetDerivations();

            return roleType;
        }

        public DynamicOneToOneRoleType AddOneToOne(DynamicObjectType roleObjectType, string roleName)
        {
            var roleType = new DynamicOneToOneRoleType(roleObjectType, roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicOneToOneAssociationType(this, roleType);
            roleObjectType.AddAssociationType(associationType);

            this.Meta.ResetDerivations();

            return roleType;
        }

        public DynamicManyToOneRoleType AddManyToOne(DynamicObjectType roleObjectType, string roleName)
        {
            var roleType = new DynamicManyToOneRoleType(roleObjectType, roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicManyToOneAssociationType(this, roleType);
            roleObjectType.AddAssociationType(associationType);

            this.Meta.ResetDerivations();

            return roleType;
        }

        public DynamicOneToManyRoleType AddOneToMany(DynamicObjectType roleObjectType, string roleName)
        {
            var roleType = new DynamicOneToManyRoleType(roleObjectType, roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicOneToManyAssociationType(this, roleType);
            roleObjectType.AddAssociationType(associationType);

            this.Meta.ResetDerivations();

            return roleType;
        }

        public DynamicManyToManyRoleType AddManyToMany(DynamicObjectType roleObjectType, string roleName)
        {
            var roleType = new DynamicManyToManyRoleType(roleObjectType, roleName);
            this.AddRoleType(roleType);

            var associationType = new DynamicManyToManyAssociationType(this, roleType);
            roleObjectType.AddAssociationType(associationType);

            this.Meta.ResetDerivations();

            return roleType;
        }

        internal void ResetDerivations()
        {
            this.derivedAssociationTypeByName = null;
            this.derivedRoleTypeByName = null;
        }

        private void AddAssociationType(IDynamicAssociationType associationType)
        {
            this.CheckNames(associationType.SingularName, associationType.PluralName);

            this.assignedAssociationTypeByName.Add(associationType.SingularName, associationType);
            this.assignedAssociationTypeByName.Add(associationType.PluralName, associationType);
        }

        private void AddRoleType(IDynamicRoleType roleType)
        {
            this.CheckNames(roleType.SingularName, roleType.PluralName);

            this.assignedRoleTypeByName.Add(roleType.SingularName, roleType);
            this.assignedRoleTypeByName.Add(roleType.PluralName, roleType);
        }

        private void CheckNames(string singularName, string pluralName)
        {
            if (this.RoleTypeByName.ContainsKey(singularName) ||
                this.AssociationTypeByName.ContainsKey(singularName))
            {
                throw new Exception($"{singularName} is not unique");
            }

            if (this.RoleTypeByName.ContainsKey(pluralName) ||
                this.AssociationTypeByName.ContainsKey(pluralName))
            {
                throw new Exception($"{pluralName} is not unique");
            }
        }
    }
}