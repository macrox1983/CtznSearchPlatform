using System;
using System.IO;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;

using BreakingStorm.Common.Primitives;

using BreakingStorm.Common.Network.UDP;
using BreakingStorm.Common.Loggers;

namespace BreakingStorm.Common.Network.UDP
{
    public class NetworkUDPSystem
    {
        private Logger _logger;

        /*private IDictionary<string, UCareUDP> connection = new Dictionary<string, UCareUDP>();
        private IDictionary<string, UdpClient> inComeConnection = new Dictionary<string, UdpClient>();
        */

        public NetworkUDPSystem(Logger logger)
        {
            this._logger = logger;                      
        }

        
        
        public UDPSocket CreateServer(int port)
        {
            UDPSocket udp = new UDPSocket(port);
            udp.Event_GetPackage += this.handlerGetPackage;
            udp.Event_SendError += this.handlerSendError;
            udp.Event_ConnectionError += this.handlerConnection;
            udp.StartListen();
            this._logger.Info(String.Format("Start UDP listener at {0}", port));
            return udp;
        }

        public UDPSocket CreateClient(string ip, int port)
        {
            UDPSocket udp = new UDPSocket(new IPEndPoint(IPAddress.Parse(ip), port));
            udp.Event_GetPackage += this.handlerGetPackage;
            udp.Event_SendError += this.handlerSendError;
            udp.Event_ConnectionError += this.handlerConnection;
            this._logger.Info(String.Format("Create UDP client for {0}:{1}", ip, port));
            return udp;
        }
        
        private void handlerGetPackage(UDPEvent args)
        {
            /*CommandBaseUDP command = this.parseCommand(args.Bytes);
            switch (command.Type)
            {
                case CommandTypeEnum.WantMakeLink:
                    CommandWantMakeLinkUDP cwml = (CommandWantMakeLinkUDP)command;                    
                    CommandMakeLinkUDP cml = new CommandMakeLinkUDP(cwml.Sid);
                    UCareUDP nClient = this.CreateClient(args.GetFromEndPoint.Address.ToString(), args.GetFromEndPoint.Port);
                    nClient.Send(cml);                    
                    break;
                case CommandTypeEnum.MakeLink:
                    args.Client.ChangeEndPoint(args.GetFromEndPoint);
                    CommandMakeLinkUDP cml2 = new CommandMakeLinkUDP(0);
                //    args.Client.Send(cml2);
                    break;
            }
            command = null;*/

        }
        private void handlerConnection(UDPEvent args)
        {
            _logger.Info(args.ToString());
        }
        private void handlerSendError(UDPEvent args)
        {
            _logger.Error(args.ToString());
        }
        /*
        private CommandBaseUDP parseCommand(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes); stream.Position = 0;
            BinaryReader reader = new BinaryReader(stream);
            CommandTypeEnum commandType = (CommandTypeEnum)reader.ReadUInt16();
            switch (commandType)
            {
                case CommandTypeEnum.WantMakeLink:
                    return CommandWantMakeLinkUDP.Parse(reader);
                case CommandTypeEnum.MakeLink:
                    return CommandMakeLinkUDP.Parse(reader);

            }
            return null;
        }
        /*

        

        public bool ConnectToHost(string ip, int port)
        {
            UdpClient client = new UdpClient();
            CommandBaseUDP command = new CommandWantMakeLinkUDP((uint)new Random().Next(100000, 999999));
            byte[] data = command.BytesList;
            int sended = client.Send(data, data.Length, ip, port);
            IPEndPoint ipEndPoint = null;
            byte[] bytes = this._server.Receive(ref ipEndPoint);
            
            client.Close();
            return false;
        }

   

        private void startListen()
        {
            if (this._server==null) this._server = new UdpClient(8001);
            IPEndPoint ip = null;
            byte[] bytes = this._server.Receive(ref ip);
            CommandBaseUDP request = this.parseCommand(bytes);            
            switch (request.Type)
            {
                case CommandTypeEnum.WantMakeLink:
                    CommandWantMakeLinkUDP cwml = (CommandWantMakeLinkUDP)request;
                    UdpClient client = new UdpClient();
                    this.inComeConnection.Add(String.Format("{0}:{1}", ip.Address, ip.Port), client);
                    CommandMakeLinkUDP cml = new CommandMakeLinkUDP(cwml.Sid);
                    byte[] data = cml.BytesList;
                    int sended = client.Send(data, data.Length, ip.Address.ToString(), ip.Port);
                    break;
            }
            this.startListen();           
        }

        private void sendMessage(IPEndPoint ip, string message)
        {
            UdpClient client = new UdpClient();            
            byte[] data = Encoding.UTF8.GetBytes(message);
            int sended = client.Send(data, data.Length, ip.Address.ToString(), ip.Port);            
            client.Close();
        }*/
    }
}
