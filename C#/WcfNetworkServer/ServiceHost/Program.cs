using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WcfNetworkServer;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ServiceHost
{
    class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8000/GettingStarted/");
            System.ServiceModel.ServiceHost selfHost = new System.ServiceModel.ServiceHost(typeof(WcfService), baseAddress);

            try
            {
                selfHost.AddServiceEndpoint(typeof (IWcfService), new WSHttpBinding(), "WcfService");

                ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
                smb.HttpGetEnabled = true;
                selfHost.Description.Behaviors.Add(smb);

                selfHost.Open();
                Console.WriteLine("The service is ready.");
                Console.WriteLine("Press <ENTER> to terminate service.");
                Console.WriteLine();
                Console.ReadLine();

                selfHost.Close();
            }
            catch(CommunicationException ce)
            {
                Console.WriteLine("An exception occured: {0}", ce.Message);
                selfHost.Abort();
            }
        }
    }
}
