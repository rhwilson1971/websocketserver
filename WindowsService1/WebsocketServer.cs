using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WindowsService1
{
    internal class WebsocketServer
    {
        public int Port { get; set; }
        public string Address {  get; set; }

        private TcpListener listener;
        private IPEndPoint endPoint;

        private List<NetClient> socketClients;

        ManualResetEvent clientConnected = new ManualResetEvent(false);

        public void Start()
        {

            endPoint = new IPEndPoint(IPAddress.Parse(Address), Port);

            listener = new TcpListener(endPoint);
            listener.Start();
            listener.BeginAcceptTcpClient(OnClientAccepted, listener);
                



            
        }

        void OnClientAccepted(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;

            TcpClient client = listener.AcceptTcpClient();

            NetClient netClient = new NetClient(client);
            socketClients.Add(netClient);

            netClient.Start();
            
        }




    }


    internal class NetClient
    {
        ManualResetEvent resetEvent = new ManualResetEvent(false);
        TcpClient _client;


        public NetClient(TcpClient client)
        {
            _client = client;

            Thread thread = new Thread(Run);

        }

        void Run()
        {
            bool keepRunning = true;
            
            do
            {
                
                NetworkStream stream = _client.GetStream();

                while (true)
                {
                    while (!stream.DataAvailable) ;
                    byte[] buffer = new byte[_client.Available];
                    stream.Read(buffer, 0, buffer.Length);

                    string data = Encoding.UTF8.GetString(buffer);
                
                    Console.WriteLine(data);
                }

            }
            while (keepRunning);
        }
    }
}
