using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CookComputing.XmlRpc;

namespace CalculatorServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    public interface ICalculatorService
    {
        [XmlRpcMethod("math.Add", Description = "add two integers")]
        int Add(int a, int b);
    }    

    public interface ICalculatorServiceProxy : ICalculatorService, IXmlRpcProxy
    {
    }
}
