using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Ziv.ServiceModel.CodeGeneration
{
    public static class ConfigGenertionUtils
    {
        public static IEnumerable<Type> GetAllServiceContractTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsServiceContract())
                    {
                        yield return type;
                    }
                }
            }
        }

        public static IEnumerable<Type> GetAllServiceTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!string.IsNullOrEmpty(type.Namespace) &&
                        !type.Namespace.StartsWith("System.") &&
                        !type.Namespace.StartsWith("Microsoft.") &&
                        type.GetInterfaces().Any(inf => inf.IsServiceContract()))
                    {
                        yield return type;
                    }
                }
            }
        }

        public static IEnumerable<Type> GetServiceTypesByRegularExpressions(string[] patterns)
        {
            var allServiceTypes = GetAllServiceTypes().ToArray();
            return patterns.SelectMany(ptn => GetServiceTypesByRegularExpression(allServiceTypes, ptn)).Distinct();
        }

        public static IEnumerable<Type> GetServiceTypesByRegularExpression(string pattern)
        {
            return GetServiceTypesByRegularExpression(GetAllServiceTypes(), pattern);
        }

        public static IEnumerable<Type> GetServiceTypesByRegularExpression(IEnumerable<Type> types, string pattern)
        {
            return types.Where(typ => Regex.IsMatch(typ.Name, pattern));
        }
    }
}
