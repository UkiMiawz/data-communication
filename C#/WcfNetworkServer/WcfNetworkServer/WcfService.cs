using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WcfNetworkServer
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in both code and config file together.
    public class WcfService: IWcfService
    {
        private string GlobalString; 

        public string AppendString(string value)
        {
            Console.WriteLine("You entered: {0}", value);
            GlobalString += value;
            return GlobalString;
        }
    }
}
