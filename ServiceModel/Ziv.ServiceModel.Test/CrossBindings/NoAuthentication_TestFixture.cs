using System;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Practices.Unity;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.ServiceModel.Channels;
using Sample.DTO;
using Sample.Operations;
using Ziv.ServiceModel.Operations.OperationsManager;
using Ziv.ServiceModel.Operations;
using System.Diagnostics;
using Ziv.ServiceModel;
using Ziv.ServiceModel.Activation;

namespace Ziv.ServiceModel.Test.CrossBindings
{
    //[TestClass]
    //public class NoAuthentication_TestFixture
    //{
    //    private const string BASIC_HTTP_CONFIG_NAME = "BasicHttp.config";
    //    private const string NAMED_PIPE_CONFIG_NAME = "NamedPipe.config";
    //    private const string WS_HTTP_CONFIG_NAME = "WsHttp.config";
    //    private const string TCP_CONFIG_NAME = "Tcp.config";

    //    [TestMethod]
    //    [DeploymentItem(BASIC_HTTP_CONFIG_NAME)]
    //    public void BasicHttp_NoIdendty_Test()
    //    {
    //        RunNoIdentityTest(BASIC_HTTP_CONFIG_NAME);
    //    }

    //    [TestMethod]
    //    [DeploymentItem(NAMED_PIPE_CONFIG_NAME)]
    //    public void NamedPipe_NoIdendty_Test()
    //    {
    //        RunNoIdentityTest(NAMED_PIPE_CONFIG_NAME);
    //    }

    //    [TestMethod]
    //    [DeploymentItem(WS_HTTP_CONFIG_NAME)]
    //    public void WsHttp_NoIdendty_Test()
    //    {
    //        RunNoIdentityTest(WS_HTTP_CONFIG_NAME);
    //    }

    //    [TestMethod]
    //    [DeploymentItem(TCP_CONFIG_NAME)]
    //    public void Tcp_NoIdendty_Test()
    //    {
    //        RunNoIdentityTest(TCP_CONFIG_NAME);
    //    }

    //    [TestMethod]
    //    public void NoCommunictions_Test()
    //    {
    //        DoSomething(new TrialsService(new SingleProcessDeploymentOperationsManager(), new SomeRequiredService(), new SomeRequiredService()));
    //    }


    //    private void RunNoIdentityTest(string configFileName)
    //    {
    //        IUnityContainer container = new UnityContainer();

    //        RegisterTypes(container);
    //        ServiceLocator.SetServiceLocator(new UnityServiceLocator(container));

    //        using (var serviceHost = new TestingServiceHost(typeof(TrialsService), configFileName))
    //        {
    //            serviceHost.Open();
    //            var filemap = new System.Configuration.ExeConfigurationFileMap();
    //            filemap.ExeConfigFilename = configFileName;

    //            System.Configuration.Configuration config =
    //                System.Configuration.ConfigurationManager.OpenMappedExeConfiguration
    //                (filemap,
    //                 System.Configuration.ConfigurationUserLevel.None);

    //            using (ConfigurationChannelFactory<ITrialsService> channelFactory =
    //                new ConfigurationChannelFactory<ITrialsService>("ClientEP", config, null))
    //            {
    //                ITrialsService channel = channelFactory.CreateChannel();
    //                DoSomething(channel);
    //            }
    //        }
    //    }

    //    private static void DoSomething(ITrialsService TrialsService)
    //    {
    //        int parameter = 73;

    //        var svc = new TrialsService((new SingleProcessDeploymentOperationsManager()), new SomeRequiredService(), new SomeRequiredService());
    //        var r = svc.DoSomething(new SomeParameters { Parameter = 23 });


    //        var someResult = TrialsService.DoSomething(new SomeParameters() { Parameter = parameter });
    //        Assert.AreEqual(
    //            string.Format(DoSomethingOperation.DO_SOMETHING_RESULT_TEMPLATE, parameter),
    //            someResult.Result);
    //    }


    //    private static void RegisterTypes(IUnityContainer container)
    //    {
    //        container.RegisterType<IOperationsManager, SingleProcessDeploymentOperationsManager>(new ContainerControlledLifetimeManager());
    //        container.RegisterType<ISomeRequiredService, SomeRequiredService>(new ContainerControlledLifetimeManager());

    //    }

    //    //  public class UnityServiceHost : ServiceHost
    //    //  {

    //    //      public UnityServiceHost(IUnityContainer container,
    //    //        Type serviceType, params Uri[] baseAddresses)
    //    //        : base(serviceType, baseAddresses)
    //    //      {
    //    //          UnityInitialization(container);
    //    //      }

    //    //      private void UnityInitialization(IUnityContainer container)
    //    //      {
    //    //          if (container == null)
    //    //          {
    //    //              throw new ArgumentNullException("container");
    //    //          }

    //    //          foreach (var implementedContract in this.ImplementedContracts.Values)
    //    //          {
    //    //              implementedContract.Behaviors.Add(new UnityInstanceProvider(container));
    //    //          }
    //    //      }
    //    //  }
    //    //  public class UnityInstanceProvider
    //    //: IInstanceProvider, IContractBehavior
    //    //  {
    //    //      private readonly IUnityContainer container;

    //    //      public UnityInstanceProvider(IUnityContainer container)
    //    //      {
    //    //          if (container == null)
    //    //          {
    //    //              throw new ArgumentNullException("container");
    //    //          }

    //    //          this.container = container;
    //    //      }

    //    //      #region IInstanceProvider Members

    //    //      public object GetInstance(InstanceContext instanceContext,
    //    //        Message message)
    //    //      {
    //    //          return this.GetInstance(instanceContext);
    //    //      }

    //    //      public object GetInstance(InstanceContext instanceContext)
    //    //      {
    //    //          return this.container.Resolve(
    //    //            instanceContext.Host.Description.ServiceType);
    //    //      }

    //    //      public void ReleaseInstance(InstanceContext instanceContext,
    //    //        object instance)
    //    //      {
    //    //      }

    //    //      #endregion

    //    //      #region IContractBehavior Members

    //    //      public void AddBindingParameters(
    //    //        ContractDescription contractDescription,
    //    //        ServiceEndpoint endpoint,
    //    //        BindingParameterCollection bindingParameters)
    //    //      {
    //    //      }

    //    //      public void ApplyClientBehavior(
    //    //        ContractDescription contractDescription,
    //    //        ServiceEndpoint endpoint, ClientRuntime clientRuntime)
    //    //      {
    //    //      }

    //    //      public void ApplyDispatchBehavior(
    //    //        ContractDescription contractDescription,
    //    //        ServiceEndpoint endpoint,
    //    //        DispatchRuntime dispatchRuntime)
    //    //      {
    //    //          dispatchRuntime.InstanceProvider = this;
    //    //      }

    //    //      public void Validate(
    //    //        ContractDescription contractDescription,
    //    //        ServiceEndpoint endpoint)
    //    //      {
    //    //      }

    //    //      #endregion
    //    //  }
    //}
}
