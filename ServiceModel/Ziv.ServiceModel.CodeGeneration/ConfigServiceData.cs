using System;
using System.Collections.Generic;

namespace Ziv.ServiceModel.CodeGeneration
{
    public class ConfigServiceData
    {
        public ConfigServiceData(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public Type ServiceType { get; private set; }

        public string BehaviorName { get; set; }

        public IEnumerable<ConfigEnpointData> Endpoints { get; set; }

        public string RelativeAddress
        {
            get
            {
                return string.Format("{0}/{1}.svc", ServiceType.Namespace.Replace(".", "/"), ServiceType.Name);
            }
        }

    }
}