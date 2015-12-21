using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WcfServiceLibrary1
{
    public class Service2: IServiceContract
    {
        public string Hello(string name)
        {
            return "Hello " + name;
        }
    }
}
