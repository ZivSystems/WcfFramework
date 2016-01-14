using System;
using Sample.DTO;
using Sample.Operations;
using Ziv.ServiceModel.Client;
using Ziv.ServiceModel.Operations.OperationsManager;

namespace Sample.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var keepRun = true;
            while (keepRun)
            {
                keepRun = Menu();
            }
            Console.WriteLine("\n\nGoodbye!");
            Console.WriteLine("Any key to exit...");
            Console.ReadKey();
        }

        private static bool Menu()
        {
            Console.Clear();
            Console.WriteLine("********************************");
            Console.WriteLine("*                              *");
            Console.WriteLine("*      Ziv.ServiceModel        *");
            Console.WriteLine("*       By Ziv Systems         *");
            Console.WriteLine("*  Sample Client Application   *");
            Console.WriteLine("*                              *");
            Console.WriteLine("********************************");
            Console.WriteLine();
            char choice;
            bool isValidChoice;
            do
            {
                Console.Write("Choose between WCF proxy (1), in-process proxy (2), or exit (3): ");
                choice = Console.ReadKey().KeyChar;
                isValidChoice = choice >= '1' && choice <= '3';
                if (!isValidChoice)
                {
                    Console.WriteLine("\nWrong typing.");
                }
            } while (!isValidChoice);
            switch (choice)
            {
                case '1':
                    UseWcfProxy();
                    break;
                case '2':
                    UseInProcessProxy();
                    break;
                case '3':
                    return false;
            }
            return true;
        }

        private static void UseWcfProxy()
        {
            var wcfCachingChannelFactoryProvider = new WcfCachingChannelFactoryProvider();
            IProxyFactory<ITrialsService> proxyFactory = new WcfProxyFactory<ITrialsService>(wcfCachingChannelFactoryProvider);
            UseProxyFactory(proxyFactory);
        }

        private static void UseInProcessProxy()
        {
            var requiredService = new SomeRequiredService();
            var trialsServiceImpl = new TrialsService(
                    new SingleProcessDeploymentOperationsManager(),
                    requiredService,
                    requiredService); 
            IProxyFactory<ITrialsService> proxyFactory = new InProcessProxyFactory<ITrialsService>(trialsServiceImpl);
            UseProxyFactory(proxyFactory);
        }

        private static void UseProxyFactory(IProxyFactory<ITrialsService> proxyFactory)
        {
            Console.WriteLine("\n\nUsing {0} as IProxyFactory.", proxyFactory.GetType().GetGenericTypeDefinition().Name);
            Console.Write("Enter a number: ");
            int parameter;
            while (!int.TryParse(Console.ReadLine(), out parameter))
            {
                Console.Write("Please enter a valid integer: ");
            }
            var proxy = proxyFactory.GetProxy();
            var parameters = new SomeParameters() { Parameter = parameter };
            var operationStatus = proxy.Execute(ch => ch.DoSomething(parameters));
            Console.WriteLine("Proxy operation result: {0}", operationStatus.Result);
            Console.Write("Press any key to continue...");
            Console.ReadKey();
        }
    }
}
