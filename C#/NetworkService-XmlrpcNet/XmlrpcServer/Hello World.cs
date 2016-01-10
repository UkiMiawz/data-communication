using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class HelloWorldService : MarshalByRefObject
{
    [XmlRpcMethod("HelloWorld.hello")]
    public string HelloWorld(string ipAddress)
    {
        Console.WriteLine("{0} ask for service", ipAddress);       
        return "Hello " + ipAddress;
    }
}
