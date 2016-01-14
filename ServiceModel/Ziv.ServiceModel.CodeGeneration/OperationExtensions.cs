using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ziv.ServiceModel.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Ziv.ServiceModel.CodeGeneration
{
    public static class OperationExtensions
    {
        public static bool IsOperationType(this Type operationType)
        {
            return typeof(IOperation).IsAssignableFrom(operationType) && operationType.OperationAttributeExists();
        }

        public static string OperationShortName(this Type operationType)
        {
            return operationType.Name.EndsWith("Operation")
                       ? operationType.Name.Substring(0, operationType.Name.Length - "Operation".Length)
                       : operationType.Name;
        }

        public static string OperationServiceClassName(this Type operationType)
        {
            return operationType.OperationAttribute().ServiceImplementationClassName;
        }

        public static string OperationContractInterfaceName(this Type operationType)
        {
            return operationType.OperationAttribute().ServiceContractInterfaceName;
        }

        public static Type OperationResultType(this Type operationType)
        {
            return operationType.GetInterfaces().Single(
                intfc => intfc.IsGenericType && intfc.GetGenericTypeDefinition() == typeof(IOperation<>)).
                GetGenericArguments().Single();
        }

        public static string ToInvocation(this IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(prm => prm.Name).ToCommaSeperatedString();
        }

        public static string ToDecleration(this IEnumerable<ParameterInfo> parameters)
        {
            return parameters.Select(prm => prm.ParameterType.Name + " " + prm.Name).ToCommaSeperatedString();
        }

        public static string ToCommaSeperatedString(this IEnumerable<string> strings)
        {

            if (!strings.Any())
            {
                return string.Empty;
            }
            if (strings.Count() == 1)
            {
                return strings.Single();
            }

            return strings.Aggregate((seed, str) => seed + ", " + str);
        }

        public static IEnumerable<ParameterInfo> OperationParameters(this Type operationType)
        {
            ConstructorInfo constructorInfo = operationType.GetConstructors().Single();
            return constructorInfo.GetParameters().TakeWhile(prm => prm.ParameterType != typeof(IOperationsManager));
        }

        public static IEnumerable<ParameterInfo> OperationServices(this Type operationType)
        {
            ConstructorInfo constructorInfo = operationType.GetConstructors().Single();
            var operationParameters = operationType.OperationParameters();
            return constructorInfo.GetParameters().Where(prm => !operationParameters.Contains(prm));
        }

        public static string OperationConstructorInvocation(this Type operationType)
        {
            ConstructorInfo constructorInfo = operationType.GetConstructors().Single();
            var operationParameters = operationType.OperationParameters().Select(prm => prm.Name);
            var operationServices = operationType.OperationServices().Select(srv => srv.ToPrivateValirableName());
            return operationParameters.Union(operationServices).ToCommaSeperatedString();
        }

        public static string ToPrivateValirableName(this ParameterInfo parameterInfo)
        {
            return string.Format("_{0}{1}", parameterInfo.Name.Substring(0, 1).ToLower(),
                                 parameterInfo.Name.Substring(1));
        }

        public static IEnumerable<string> OperationServiceNamespaces(this Type operationType)
        {
            List<string> nameSpaces = new List<string>();
            nameSpaces.AddIfNotIncluded("System", operationType);
            nameSpaces.AddIfNotIncluded("Ziv.ServiceModel", operationType);
            nameSpaces.AddIfNotIncluded("Ziv.ServiceModel.Operations", operationType);
            nameSpaces.AddIfNotIncluded(operationType.OperationResultType().Namespace, operationType);
            foreach (var constructorParam in operationType.GetConstructors().Single().GetParameters())
            {
                nameSpaces.AddIfNotIncluded(constructorParam.ParameterType.Namespace, operationType);

            }
            return nameSpaces;
        }

        public static IEnumerable<string> OperationContractNamespaces(this Type operationType)
        {
            List<string> nameSpaces = new List<string>();
            nameSpaces.AddIfNotIncluded("System", operationType);
            nameSpaces.AddIfNotIncluded("Ziv.ServiceModel.Operations", operationType);
            nameSpaces.AddIfNotIncluded(operationType.OperationResultType().Namespace, operationType);
            foreach (var constructorParam in operationType.OperationParameters())
            {
                nameSpaces.AddIfNotIncluded(constructorParam.ParameterType.Namespace, operationType);
            }
            foreach (var faultContract in operationType.OperationFaultContracts())
            {
                nameSpaces.AddIfNotIncluded(faultContract.Namespace, operationType);
            }
            return nameSpaces;
        }

        public static void AddIfNotIncluded(this List<string> list, string element, Type operationType)
        {
            if (operationType.Namespace == element)
            {
                return;
            }
            if (!list.Contains(element))
            {
                list.Add(element);
            }
        }

        public static bool OperationAttributeExists(this Type operationType)
        {
            return operationType.GetCustomAttributes(typeof(OperationAttribute), false).Any();
        }

        public static OperationAttribute OperationAttribute(this Type operationType)
        {
            return (OperationAttribute)operationType.GetCustomAttributes(typeof(OperationAttribute), false).Single();
        }

        public static Type[] OperationFaultContracts(this Type operationType)
        {
            return operationType.GetCustomAttributes(typeof(OperationFaultContractAttribute), true)
                .Cast<OperationFaultContractAttribute>().Select(att => att.DetailType).ToArray();
        }

        public static bool OperationIsGeneratingSyncMethod(this Type operationType)
        {
            return (operationType.OperationAttribute().Generate & OperationGeneration.Sync) == OperationGeneration.Sync;
        }

        public static bool OperationIsReturningVoid(this Type operationType)
        {
            return (operationType.OperationResultType() == typeof(VoidResult));
        }

        public static bool OperationIsGeneratingAsyncMethods(this Type operationType)
        {
            return (operationType.OperationAttribute().Generate & OperationGeneration.Async) == OperationGeneration.Async;
        }
    }
}