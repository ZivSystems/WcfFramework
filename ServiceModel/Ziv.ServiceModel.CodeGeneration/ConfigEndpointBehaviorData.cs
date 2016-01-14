using System.Xml.Linq;

namespace Ziv.ServiceModel.CodeGeneration
{
    public class ConfigEndpointBehaviorData
    {
        public string Name { get; set; }
        public XElement Content { get; set; }
    }
}