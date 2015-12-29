using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using CookComputing.XmlRpc;

namespace CalculatorServiceLibrary
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class CalculatorService : XmlRpcService, ICalculatorService
    {        
        public int Add(int a, int b)
        {
            return a + b;
        }                
    }
}
