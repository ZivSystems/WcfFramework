using System.Xml.Linq;

namespace Ziv.ServiceModel.CodeGeneration
{
    public class ConfigServiceBehaviorData
    {
        public string Name { get; set; }
        public XElement Content { get; set; }
    }
}