using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CookComputing.XmlRpc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

class _
{
    static void Main(string[] args)
    {
        bool bUseSoap = false;
        if (args.Length > 0 && args[0] == "SOAP")
            bUseSoap = true;

        HttpChannel chnl;
        if (bUseSoap)
            chnl = new HttpChannel();
        else
            chnl = new HttpChannel(null, new XmlRpcClientFormatterSinkProvider(), null);
        ChannelServices.RegisterChannel(chnl, false);

        string localServer = "http://localhost:5678/calculator.rem";
        INetworkServer calc = (INetworkServer)Activator.GetObject(
            typeof(INetworkServer), localServer);

        try
        {
            string result = calc.AppendString("Aderick");
            Console.WriteLine("the result is {0}", result);
            Console.ReadLine();
        }
        catch (XmlRpcFaultException fex)
        {
            Console.WriteLine("Fault response {0} {1} {2}",
                fex.FaultCode, fex.FaultString, fex.Message);
        }

    }
}
