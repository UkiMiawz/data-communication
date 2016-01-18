using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface ICSharpRpcClient : IXmlRpcProxy
{
    [XmlRpcMethod("Hello")]
    string HelloWorld(string input);
}

