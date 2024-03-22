using System;
using System.Collections.Generic;
using Allors.Dynamic.Meta;

namespace Allors.Dynamic.Binding
{
    public class DynamicPopulation(DynamicMeta meta) : Dynamic.DynamicPopulation(meta)
    {
        protected override IDynamicObject NewObject(DynamicObjectType @class)
        {
            return new DynamicObject(this, @class);
        }

        public new dynamic New(DynamicObjectType @class, params Action<dynamic>[] builders)
        {
            return base.New(@class, builders);
        }

        public new dynamic New(string className, params Action<dynamic>[] builders)
        {
            return base.New(className, builders);
        }

        public new IEnumerable<dynamic> Objects => base.Objects;
    }
}