using System;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Collections.Generic;
using EncryptionClass;
using System.Numerics;

namespace SimpleCallResponseServer
{
    class Program
    {
        public static Encryption Encrypt = new Encryption();
        public static byte PrivateKey = 15;
        public static BigInteger pkey = 9081230891108293;
        public static byte[] key;
        public static BigInteger pgkey = 13;
        public static BigInteger gkey = 12;
        public static BigInteger nkey = 11231231232;
        


        public static List<TcpClient> clients = new List<TcpClient>();
        
        static void Main(string[] args)
        {
            int port = 13356;
            IPAddress ip = IPAddress.Any;
            IPEndPoint localEndpoint = new IPEndPoint(ip, port);

            TcpListener listener = new TcpListener(localEndpoint);
            listener.Start();

            AcceptClients(listener);

            bool isRunning = true;
            while (isRunning)
            {
                Console.WriteLine("Skriv");
                string message = Console.ReadLine();
                byte[] buffer = Encoding.UTF8.GetBytes(message);
                Encrypt.EncryptByte(buffer, key);

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

        //handshake
        //serveren sender n g og ga
        //client modtager og sender bg tilbage
        //serveren tager bg og kombinere den med a

        public static async void AcceptClients(TcpListener listener)
        {
            while (true)
            {
                //Opretter conn
                TcpClient client = await listener.AcceptTcpClientAsync();
                clients.Add(client);
                NetworkStream stream = client.GetStream();

                byte[] keybuffer = EncryptionClass.Encryption.CreatePublicKey(pkey, gkey, nkey);
                stream.Write(keybuffer, 0, keybuffer.Length);

                byte[] buffer = new byte[256];
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);

                byte[] keyfromclient = new byte[read];
                Array.Copy(buffer, 0, keyfromclient, 0, read);
                key = EncryptionClass.Encryption.CreatePrivateKey(pkey, new BigInteger(keyfromclient), nkey);
                Console.WriteLine(key);
                RecieveMessages(stream);
            }
        }

        public static async void RecieveMessages(NetworkStream stream)
        {
            byte[] buffer = new byte[256];
            while (true)
            {
                int read = await stream.ReadAsync(buffer, 0, buffer.Length);
                Encrypt.DecryptByte(buffer, key);
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
