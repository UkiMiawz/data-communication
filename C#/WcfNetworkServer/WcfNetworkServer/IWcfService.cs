using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;


namespace WcfNetworkServer
{
    [ServiceContract]
    public interface IWcfService 
    {
        [OperationContract(Action = "WcfService.AppendString")]
        string AppendString(string value);       
    }   
}
