using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BreakingStorm.Common.Network.UDP
{
    public class UDPSocket
    {
        public delegate void UDPEventDelegate(UDPEvent args);
        

        public event UDPEventDelegate Event_GetPackage;
        public event UDPEventDelegate Event_RecieveBrokenPackage;
        public event UDPEventDelegate Event_SendError;
        public event UDPEventDelegate Event_ConnectionError;
         

        private readonly object locker = new object();

        private UdpClient _client;
        private Thread _listenThread;
             
        /// <summary>
        /// Создает сервер по определенному порту
        /// </summary>
        /// <param name="port"></param>
        public UDPSocket(int port)
        {
            this._client = new UdpClient(port);
            this._client.DontFragment = true;
            this._client.EnableBroadcast = true;
        }

        /// <summary>
        /// Создает клиент который будет конектиться к определенному адресу
        /// </summary>
        /// <param name="endpoint"></param>
        public UDPSocket(IPEndPoint endpoint)
        {
            this._client = new UdpClient();
            this._client.DontFragment = true;
            this._client.EnableBroadcast = true;
            this.EndPoint = endpoint;
        }

        public IPEndPoint EndPoint { get; private set; }
        public int Port { get; private set; }

        public void ChangeEndPoint(IPEndPoint endpoint)
        {
            lock (this.locker)
                this.EndPoint = endpoint;
        }
        public void StartListen()
        {
            lock (this.locker)
            {
                if (this._listenThread != null) throw new Exception("Already listen");
                this._listenThread = new Thread(this.listen);
                this._listenThread.Start();
            }
        }
        public void Send(UDPPackage package)
        {
            lock (this.locker)
            {                
                byte[] bytes = package.Body;
                try
                {
                    int sended = this._client.SendAsync(bytes, bytes.Length, this.EndPoint).GetAwaiter().GetResult();                                        
                    if (sended != bytes.Length)
                    {
                        this.Event_SendError?.Invoke(new UDPEvent(this, package));
                        return;
                    }
                    if (this._listenThread == null) this.StartListen();
                } catch (Exception e)
                {
                    this.Event_ConnectionError?.Invoke(new UDPEvent(this, e));
                    return;
                }
            }
        }        
        private void listen()
        {
            try
            {
                UdpReceiveResult result = this._client.ReceiveAsync().GetAwaiter().GetResult();
                byte[] bytes = result.Buffer;
                UDPPackage package = UDPPackage.Parse(bytes);
                IPEndPoint ip = result.RemoteEndPoint;
                if (!package.Broken)                
                    this.Event_GetPackage?.Invoke(new UDPEvent(this, package, ip)); else                
                    this.Event_RecieveBrokenPackage?.Invoke(new UDPEvent(this, package, ip));                

            } catch (Exception e)
            {
                lock (this.locker) this._listenThread = null;                
                this.Event_ConnectionError?.Invoke(new UDPEvent(this, e));                
                return;
            }
            listen();
        }

    }
}
