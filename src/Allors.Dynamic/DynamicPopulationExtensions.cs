namespace Allors.Dynamic
{
    using System;
    using Allors.Dynamic.Meta;

    public static class DynamicPopulationExtensions
    {
        public static Func<T, Action<dynamic>> Set<T>(this DynamicPopulation population, DynamicRoleType roleType)
        {
            Action<dynamic> action(T value)
            {
                return (obj) => ((DynamicObject)obj).Set(roleType, value);
            }

            return action;
        }

        public static Func<dynamic, Action<dynamic>> Add(this DynamicPopulation population, DynamicRoleType roleType)
        {
            var meta = population.Meta;

            Action<dynamic> action(dynamic role)
            {
                return (obj) => ((DynamicObject)obj).Add(roleType, role);
            }

            return action;
        }

        public static Func<dynamic, Action<dynamic>> Remove(this DynamicPopulation population, DynamicRoleType roleType)
        {
            var meta = population.Meta;

            Action<dynamic> action(dynamic role)
            {
                return (obj) => ((DynamicObject)obj).Remove(roleType, role);
            }

            return action;
        }
    }
}
