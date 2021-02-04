using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using EncryptionClass;

namespace SimpleCallResponseServer
{
    class Program
    {
        public static Encryption Encrypt = new Encryption();
        public static byte PrivateKey = 15;

        public static List<TcpClient> clients = new List<TcpClient>();
        static void Main(string[] args)
        {
            int port = 13356;
            IPAddress ip = IPAddress.Any;
            IPEndPoint localEndpoint = new IPEndPoint(ip, port);

            TcpListener listener = new TcpListener(localEndpoint);
            listener.Start();

            //Console.WriteLine("Så venter vi jo bar\'");
            AcceptClients(listener);

            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("Skriv");
                string message = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                Encrypt.EncryptByte(buffer, PrivateKey);

                foreach (TcpClient client in clients)
                {
                    client.GetStream().Write(buffer, 0, buffer.Length);
                }

                if (message.ToLower() == "c")
                {
                    Console.WriteLine("Conn closed");
                    break;
                }
            }
        }

        public static async void AcceptClients(TcpListener listener)
        {
            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                clients.Add(client);
                NetworkStream stream = client.GetStream();
                RecieveMessages(stream);
            }
        }

        public static async void RecieveMessages(NetworkStream stream)
        {
            byte[] buffer = new byte[256];
            while (true)
            {
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                Encrypt.DecryptByte(buffer, PrivateKey);
                string text = Encoding.UTF8.GetString(buffer, 0, read);
                Console.WriteLine($"Client writes: {text}");
                if (text.ToLower() == "c")
                {
                    break;
                }
            }
        }
    }
}
