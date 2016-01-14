using System;
using System.Linq;
using System.ServiceModel;

namespace Ziv.ServiceModel.CodeGeneration
{
    public static class ConfigExtensions
    {
        public static bool IsServiceContract(this Type type)
        {
            return type.IsInterface && type.GetCustomAttributes(typeof(ServiceContractAttribute), false).Any();
        }
    }
}
