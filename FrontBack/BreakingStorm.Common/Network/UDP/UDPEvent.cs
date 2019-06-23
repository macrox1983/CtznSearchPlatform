using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Network.UDP
{
    public class UDPEvent
    {
        public UDPEvent(UDPSocket client)
        {
            this.Client = client;            
        }

        public UDPEvent(UDPSocket client, UDPPackage package, IPEndPoint getFromEndPoint)
        {
            this.Client = client;
            this.Package = package;
            this.GetFromEndPoint = getFromEndPoint;
        }

        public UDPEvent(UDPSocket client, UDPPackage package)
        {
            this.Client = client;
            this.Package = package;
        }

        public UDPEvent(UDPSocket client, Exception exception)
        {
            this.Client = client;
            this.Exception = exception;
        }

        public UDPSocket Client { get; private set; }
        public IPEndPoint GetFromEndPoint { get; private set; } = null;
        public UDPPackage Package { get; private set; } = null;
        public Exception Exception { get; private set; } = null;
    }
}
