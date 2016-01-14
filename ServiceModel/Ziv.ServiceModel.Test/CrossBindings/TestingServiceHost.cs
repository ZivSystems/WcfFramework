using System;
using System.Collections.Generic;
using System.Linq;
using Ziv.ServiceModel.Activation;
using System.ServiceModel;
using System.Text;

namespace Ziv.ServiceModel.Test.CrossBindings
{
    //class TestingServiceHost : ZivServiceHost
    //{
    //    private static string _configFileName;

    //    public TestingServiceHost(Type serviceType, string configFileName)
    //        : base(GetStrviceTypeAndSetConfigFileName(serviceType, configFileName))
    //    {
    //    }

    //    private static Type GetStrviceTypeAndSetConfigFileName(Type serviceType, string configFileName)
    //    {
    //        _configFileName = configFileName;
    //        return serviceType;
    //    }

    //    private void LoadConfigurationFromFile(string configFileName)
    //    {
    //        var filemap = new System.Configuration.ExeConfigurationFileMap();
    //        filemap.ExeConfigFilename = configFileName;

    //        System.Configuration.Configuration config =
    //            System.Configuration.ConfigurationManager.OpenMappedExeConfiguration
    //                (filemap,
    //                 System.Configuration.ConfigurationUserLevel.None);

    //        var serviceModel = System.ServiceModel.Configuration.ServiceModelSectionGroup.GetSectionGroup(config);

    //        bool loaded = false;
    //        foreach (System.ServiceModel.Configuration.ServiceElement se in serviceModel.Services.Services)
    //        {
    //            if (!loaded)
    //                if (se.Name == this.Description.ConfigurationName)
    //                {
    //                    base.LoadConfigurationSection(se);
    //                    loaded = true;
    //                }
    //        }
    //        if (!loaded)
    //            throw new ArgumentException("ServiceElement doesn't exist");
    //    }

    //    protected override void ApplyConfiguration()
    //    {
    //        LoadConfigurationFromFile(_configFileName);
    //    }
    //}
}
