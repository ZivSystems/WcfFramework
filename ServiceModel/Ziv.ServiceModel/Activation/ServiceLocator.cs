using System;
using Ziv.ServiceModel.Utilities;

namespace Ziv.ServiceModel.Activation
{
    public static class ServiceLocator
    {
        private static IServiceProvider _current;

        public static IServiceProvider Current
        {
            get { return _current; }
        }

        public static void SetServiceLocator(IServiceProvider serviceLocator)
        {
            Logger.LogEvent(string.Format("Setting instance of {0} as a service locator", serviceLocator.GetType().Name), null, ImportanceLevels.gUnimportant);
            _current = serviceLocator;
            Logger.LogEvent(string.Format("Instance of {0} set as a service locator", serviceLocator.GetType().Name), null, ImportanceLevels.gUnimportant);
        }
    }
}
