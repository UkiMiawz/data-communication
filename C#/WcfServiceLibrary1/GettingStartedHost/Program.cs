using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using WcfServiceLibrary1;
using System.ServiceModel.Description;

namespace GettingStartedHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8000/GettingStarted");
            ServiceHost selfHost = new ServiceHost(typeof(Service2), baseAddress);

            try
            {
                selfHost.AddServiceEndpoint(typeof(IServiceContract), new WSHttpBinding(), "HelloWorldService");

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);

                selfHost.Open();
                Console.WriteLine("the service is ready.");
                Console.WriteLine("Press <enter> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                selfHost.Close();
            }
            catch(CommunicationException ex)
            {
                Console.WriteLine("An exception occured: {0}", ex.Message);
                selfHost.Abort();
            }
        }
    }
}
