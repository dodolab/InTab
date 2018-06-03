using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace InteractiveTable.Core.ClientServer
{
    public class TcpServer
    {
        #region Fields

        private Thread servingThread;
        private TcpListener listener;
        private List<TcpClient> clients;
        private readonly object locker = new object();

        #endregion

        public Action OnClientConnectedAction { get; set; }

        /// <summary>
        /// Initializes a new instance of the TcpServer class.
        /// </summary>
        public TcpServer()
        {
            clients = new List<TcpClient>();
        }

        /// <summary>
        /// Runs the server on an address:port.
        /// </summary>
        public void Run(string address, int port)
        {
            listener = new TcpListener(IPAddress.Parse(address), port);
            Console.WriteLine("Server started at IP address " + address);
            servingThread = new Thread(StartListening);
            servingThread.IsBackground = true; 
            servingThread.Start();

            Trace.WriteLine("Server started.");
        }

        public void Abort()
        {
            servingThread.Abort();
        }

        /// <summary>
        /// Starts accepting clients.
        /// </summary>
        private void StartListening()
        {
            try
            {
                listener.Start();
            }
            catch (SocketException e)
            {
                listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 11011);
                listener.Start();
            }

            while (true)
            {
                var client = listener.AcceptTcpClient(); // blocking until a client is connected
                // manage the client in a new background thread
                Task.Factory.StartNew(() =>
                {
                    lock (locker)
                    {
                        clients.Add(client);
                    }

                    Trace.Write(string.Format("New client connected from {0}.", (client.Client.RemoteEndPoint as IPEndPoint).Address));

                    if (OnClientConnectedAction != null)
                        OnClientConnectedAction.Invoke();
                }
                );
            }
        }

        /// <summary>
        /// Waits while have no clients.
        /// </summary>
        public void wait()
        {
            Console.WriteLine("Waiting for clients...");
            while (clients.Count == 0)
            {
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        public void CloseConnections()
        {
            while (clients.Count > 0)
            {
                var tmp = clients.First();
                clients.Remove(tmp);
                tmp.Close();
                Trace.Write(string.Format("Client {0} was disconnected. ", tmp.ToString()));
            }

            Trace.WriteLine("No clients connected.");
        }

        /// <summary>
        /// Sends a message to all connected clients.
        /// </summary>
        public void Broadcast(string message)
        {
            if (clients.Count < 1)
            {
                Trace.WriteLine("No clients connected.");
                return;
            }

            List<TcpClient> currentClients = new List<TcpClient>();
            currentClients.AddRange(clients);

            foreach (var client in currentClients)
                SendMessage(message, client);
        }

        /// <summary>
        /// Sends a message to a last connected client.
        /// </summary>
        /// <param name="message"></param>
        public void SendMessageToNewClient(string message)
        {
            List<TcpClient> currentClients = new List<TcpClient>();
            currentClients.AddRange(clients);
            try
            {
                SendMessage(message, currentClients.Last());
            }
            catch
            {
            }
        }

        /// <summary>
        /// Sends object serialized to XML to all connected clients.
        /// </summary>
        public void BroadcastSerializedToXML(Object objToSerialize)
        {
            if (clients.Count < 1)
            {
                Trace.WriteLine("No clients connected.");
                return;
            }
            List<TcpClient> currentClients = new List<TcpClient>();
            currentClients.AddRange(clients);

            foreach (var client in currentClients)
                SendSerializedToXML(objToSerialize, client);
        }

        /// <summary>
        /// Sends a message to a client.
        /// </summary>
        private void SendMessage(string message, TcpClient client)
        {
            try
            {
                var encoder = new UTF8Encoding();
                var buffer = encoder.GetBytes(message + Environment.NewLine);
                var stream = client.GetStream();

                stream.Write(buffer, 0, buffer.Length);
                Trace.WriteLine("Message sent.");
            }
            catch (IOException ex)
            {
                Trace.WriteLine("Client was disconnected.");
                clients.Remove(client);
            }
        }

        /// <summary>
        /// Sends a xml to a client.
        /// </summary>
        private void SendSerializedToXML(Object objToSerialize, TcpClient client)
        {
            try
            {
                var xs = new XmlSerializer(objToSerialize.GetType());
                var buffer = new ASCIIEncoding().GetBytes(Environment.NewLine);
                var stream = client.GetStream();

                xs.Serialize(stream, objToSerialize);

                stream.Write(buffer, 0, buffer.Length);
                Trace.WriteLine("Xml sended.");
            }
            catch (IOException ex)
            {
                Trace.WriteLine("Client was disconnected.");
                clients.Remove(client);
            }
            catch (Exception e)
            {
                clients.Remove(client);
            }
        }
    }
}
