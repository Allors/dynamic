using Allors.Dynamic.Meta;
using System;
using Xunit;

namespace Allors.Dynamic.Tests
{
    public class DynamicObjectTypeTests
    {
        [Fact]
        public void SameUnitTypeName()
        {
            var meta = new DynamicMeta();
            var c1 = meta.AddClass("C1");
            var c2 = meta.AddClass("C2");
            meta.AddUnit<string>(c1, "Same");
            meta.AddUnit<string>(c2, "Same");

            var population = new DynamicPopulation(meta);

            var c1a = population.New(c1, v =>
            {
                v.Same = "c1";
            });

            var c2a = population.New(c2, v =>
            {
                v.Same = "c2";
            });

            Assert.Equal("c1", c1a.Same);
            Assert.Equal("c2", c2a.Same);
        }
    }
}
