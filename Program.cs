using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;

namespace sockettest
{
    class Sockinator
    {
        Dictionary<string, string> server_config = new Dictionary<string, string>()
        {
            { "ip", "IP/DNS GOES HERE" },
            { "port", "PORT GOES HERE" },
        };

        private Socket socket;

        public void Connect()
        {
            try
            {
                IPAddress addr = GetAddress();

                IPEndPoint ep = new IPEndPoint(addr, int.Parse(server_config["port"]));
                Console.WriteLine("Created endpoint");

                socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Console.WriteLine("Created socket");

                socket.BeginConnect(ep, ConnectionCallback, socket);
                Console.WriteLine("Connected to server");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        private IPAddress GetAddress()
        {
                IPAddress[] addr = Dns.GetHostAddresses(server_config["ip"]);
                return addr[0];
        }

        private string getMessage()
        {
            Console.Write("Payload: ");
            return Console.ReadLine();
        }

        private void ConnectionCallback(IAsyncResult ar)
        {
            try
            {
                socket.EndConnect(ar);

                byte[] data = System.Text.Encoding.UTF8.GetBytes(getMessage());
                socket.Send(data);
                Console.WriteLine("Sent message");
                byte[] buffer = new byte[1024];
                socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, socket);
                Console.WriteLine("Receiving data...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }

        }
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                Socket receivedSocket = (Socket)ar.AsyncState;

                int bytesRead = receivedSocket.EndReceive(ar);

                byte[] buffer = new byte[1024];
                receivedSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, receivedSocket);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            Sockinator sock = new Sockinator();
            sock.Connect();
            Console.ReadLine();
        }
    }
}
