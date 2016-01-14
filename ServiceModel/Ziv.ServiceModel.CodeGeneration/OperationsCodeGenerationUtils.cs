using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Ziv.ServiceModel.CodeGeneration
{
    public static class OperationsCodeGenerationUtils
    {
        public static string WsdlPortTypeNamespace { get; set; }

        public static string GetServiceContractDecoration()
        {
            var serviceContractDecoration = "ServiceContract";
            if (!string.IsNullOrEmpty(WsdlPortTypeNamespace))
            {
                serviceContractDecoration = string.Format("{0}(Namespace=\"{1}\")", serviceContractDecoration, WsdlPortTypeNamespace);
            }
            serviceContractDecoration = string.Format("[{0}]", serviceContractDecoration);
            return serviceContractDecoration;
        }

        public static IEnumerable<Type> GetOperationTypesByRegularExpressions(string[] patterns)
        {
            var allOperationTypes = GetAllOperationTypes().ToArray();
            return patterns.SelectMany(ptn => GetOperationTypesByRegularExpression(allOperationTypes, ptn)).Distinct();
        }

        public static IEnumerable<Type> GetOperationTypesByRegularExpression(string pattern)
        {
            return GetOperationTypesByRegularExpression(GetAllOperationTypes(), pattern);
        }

        public static IEnumerable<Type> GetOperationTypesByRegularExpression(IEnumerable<Type> types, string pattern)
        {
            return types.Where(typ => Regex.IsMatch(typ.Name, pattern));
        }

        public static IEnumerable<Type> GetAllOperationTypes()
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (type.IsOperationType())
                    {
                        yield return type;
                    }
                }
            }
        }

        public static IEnumerable<ServiceClassData> GetOperationServiceClasses()
        {
            return GetOperationServiceClasses(GetAllOperationTypes());
        }

        public static IEnumerable<ServiceClassData> GetOperationServiceClasses(IEnumerable<Type> operationTypes)
        {
            return operationTypes.GroupBy(ot => ot.Namespace + ot.OperationServiceClassName()).Select(
                grp => new ServiceClassData()
                           {
                               ClassName = grp.First().OperationServiceClassName(),
                               ClassNamespace = grp.First().Namespace,
                               OperationsTypes = grp,
                           }
                );
        }

        public static IEnumerable<ContractInterfaceData> GetOperationContactInterfaces()
        {
            return GetAllOperationTypes().GroupBy(ot => ot.Namespace + ot.OperationContractInterfaceName()).Select(
                grp => new ContractInterfaceData()
                            {
                                InterfaceName = grp.First().OperationContractInterfaceName(),
                                InterfaceNamespace = grp.First().Namespace,
                                OperationsTypes = grp,
                            }
                );
        }


        public class ServiceClassData
        {
            public IEnumerable<Type> OperationsTypes;
            public string ClassName { get; set; }
            public string ClassNamespace { get; set; }
            public string ImplementedServiceInterfaces
            {
                get
                {
                    return OperationsTypes.Select(ot => ot.OperationContractInterfaceName())
                        .Distinct().ToCommaSeperatedString();
                }
            }
            public IEnumerable<ParameterInfo> RequierdServices
            {
                get
                {
                    var result = new List<ParameterInfo>();
                    var parameters = OperationsTypes.SelectMany(ot => ot.OperationServices());
                    foreach (var parameterInfo in parameters)
                    {
                        if (!result.Any(prm => prm.Name == parameterInfo.Name && prm.ParameterType == parameterInfo.ParameterType))
                        {
                            result.Add(parameterInfo);
                        }
                    }
                    return result;
                }
            }

            public IEnumerable<string> RequiredNamespaces
            {
                get
                {
                    return OperationsTypes.SelectMany(ot => ot.OperationServiceNamespaces()).Distinct();
                }
            }

            public string ServiceBaseClassName
            {
                get
                {
                    // This is hard coding a specific base class name. 
                    // todo: collect base service type information from Operation attributes and create base class accordingly
                    return OperationsTypes.Any(oprTyp => oprTyp.BaseType.Namespace == "ZivInterface.Web.ServiceModel")
                               ? "ZivInterface.Web.ServiceModel.ZivInterfaceServiceBase"
                               : "ServiceBase";
                }
            }
        }


        public class ContractInterfaceData
        {
            public IEnumerable<Type> OperationsTypes;
            public string InterfaceName { get; set; }
            public string InterfaceNamespace { get; set; }
            public IEnumerable<string> RequiredNamespaces
            {
                get
                {
                    return 
                        OperationsTypes.SelectMany(ot => ot.OperationContractNamespaces()).Distinct();
                }
            }
        }
    }
}
