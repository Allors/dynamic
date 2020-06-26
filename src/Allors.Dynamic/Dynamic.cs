namespace Allors.Dynamic
{
    using System;
    using Allors.Dynamic.Meta;

    public sealed class Dynamic<T> : IDynamic
    {
        public Dynamic(T type, dynamic instance)
        {
            this.Type = type;
            this.Instance = instance;
        }

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

        public static implicit operator System.Dynamic.DynamicObject(Dynamic<T> reference) => reference.Instance;

        public static implicit operator DynamicObject(Dynamic<T> reference) => reference.Instance;

        public Dynamic<T> Apply(Func<T, Action<dynamic>> setter)
        {
            setter(this.Type)(this.Instance);

            return this;
        }

        public Dynamic<T> Apply(params Func<T, Action<dynamic>>[] setters)
        {
            foreach (var setter in setters)
            {
                setter(this.Type)(this.Instance);
            }

            return this;
        }
    }
}
