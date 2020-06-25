namespace Allors.Dynamic
{
    using System.Runtime.CompilerServices;

    internal class Interned
    {
        private readonly ConditionalWeakTable<object, object> interned;

        internal Interned()
        {
            this.interned = new ConditionalWeakTable<object, object>();
        }

        internal void Intern(object @object)
        {
            this.interned.Add(@object, null);
        }
    }
}
