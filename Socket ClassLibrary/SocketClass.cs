using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Socket_ClassLibrary
{
    public class SocketClass
    {
        public Socket _Socket { get; set; }
        public string _Name { get; set; }
        public SocketClass(Socket socket)
        {
            this._Socket = socket;
        }
    }
}
