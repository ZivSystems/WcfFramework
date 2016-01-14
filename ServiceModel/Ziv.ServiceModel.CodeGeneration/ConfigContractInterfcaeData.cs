using System;

namespace Ziv.ServiceModel.CodeGeneration
{
    public class ConfigContractInterfcaeData
    {
        public Type ContractType { get; set; }

        public ConfigContractInterfcaeData(Type contractType)
        {
            ContractType = contractType;
        }
    }
}