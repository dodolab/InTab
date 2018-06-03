using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using InteractiveTable.Settings;
using InteractiveTable.Accessories;
using Emgu.CV.Structure;
using Emgu.CV;

namespace InteractiveTable.Core.ClientServer
{
    public class ServerSender
    {
        private String actualIPAddress;
        private TcpServer server;
        public const int PORT_IMAGES = 11111;
        public const int PORT_ROCKS = 11011;
        private bool sendImages;

        public ServerSender()
        {
        }

        public void ReinitializeServer()
        {
            actualIPAddress = CaptureSettings.Instance().SERVER_IP_ADDRESS;
            Console.WriteLine("Reinicializuji server na adrese "+actualIPAddress);
            if (server != null) server.Abort();
            server = new TcpServer();
            int port = CaptureSettings.Instance().SEND_IMAGES ? PORT_IMAGES : PORT_ROCKS;
            sendImages = CaptureSettings.Instance().SEND_IMAGES;
            server.Run(actualIPAddress, port);
        }


        public void SendRocks(RockList list)
        {
            //Console.WriteLine("Zkousim posilat kameny");
            if (server == null || !actualIPAddress.Equals(CaptureSettings.Instance().SERVER_IP_ADDRESS) 
                || sendImages != CaptureSettings.Instance().SEND_IMAGES)
            {
                ReinitializeServer();
            }
           // Console.WriteLine("Creating rocks");
            server.BroadcastSerializedToXML(list);
            server.SendMessageToNewClient("#end");
        }
    }
}