namespace Ziv.ServiceModel.CodeGeneration
{
    public class ConfigEnpointData
    {
        public string Contract { get; private set; }
        public string Binding { get; private set; }
        public string Address { get; private set; }

        public ConfigEnpointData(string contract, string binding, string address)
        {
            Contract = contract;
            Binding = binding;
            Address = address;
        }
    }
}