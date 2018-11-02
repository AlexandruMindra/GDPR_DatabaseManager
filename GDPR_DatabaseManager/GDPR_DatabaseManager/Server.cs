using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;


namespace GDPR_DatabaseManager
{
    class Server
    {
        Socket ServerSocket = null;
        int Port = 99;
        int Backlog = 20;
        List<Socket> Clients = new List<Socket>();
        Byte[] Buffer = new Byte[1024];

        public Server()
        {
            ServerSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Start()
        {
            ServerSocket.Bind(new IPEndPoint(IPAddress.Any, Port));
            ServerSocket.Listen(Backlog);
            Accept();
        }

        public void Accept()
        {
            ServerSocket.BeginAccept(new AsyncCallback(AcceptCallback), null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket client = ServerSocket.EndAccept(ar);
            Clients.Add(client);
            client.BeginReceive(Buffer,0,Buffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), client);
            Accept();
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            Socket client = (Socket)ar.AsyncState;
            int size = client.EndReceive(ar);
            Byte[] RecivedBytes = new Byte[size];
            Array.Copy(Buffer, RecivedBytes, size);
            String data = Encoding.ASCII.GetString(RecivedBytes);
            Console.WriteLine(data);
            //client.BeginSend();
        }
    }
}
