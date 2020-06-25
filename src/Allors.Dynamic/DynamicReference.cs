namespace Allors.Dynamic
{
    using System;
    using Allors.Dynamic.Meta;

    public struct DynamicReference<T> : IDynamicReference
    {
        public DynamicReference(T type, dynamic instance)
        {
            this.Type = type;
            this.Instance = instance;
        }

        public static implicit operator System.Dynamic.DynamicObject(DynamicReference<T> reference) => reference.Instance;

        public static implicit operator DynamicObject(DynamicReference<T> reference) => reference.Instance;

        public dynamic this[DynamicRoleType roleType]
        {
            get => this.Instance[roleType];
            set => this.Instance[roleType] = value;
        }

        public dynamic this[DynamicAssociationType associationType]
        {
            get => this.Instance[associationType];
            set => this.Instance[associationType] = value;
        }

        public T Type { get; }

        public dynamic Instance { get; }

        public void Apply(Func<T, Action<dynamic>> setter)
        {
            setter(this.Type)(this.Instance);
        }

        public void Apply(params Func<T, Action<dynamic>>[] setters)
        {
            foreach (var setter in setters)
            {
                setter(this.Type)(this.Instance);
            }
        }
    }
}
