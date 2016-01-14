using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Ziv.ServiceModel.CodeGeneration.Test
{
    [TestClass]
    public class ConfigGeneration_TestFixture
    {
        [TestMethod]
        public void CollectServiceInterfaces_Test()
        {
            new ConfigGenerationData(new[] { ".*" }, ConfigurationStrategy.Http);
        }
    }
}
