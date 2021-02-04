﻿using System;
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
        public static byte privatekey2 = 23;
        //public static byte norKey = 13;
        //public static byte genKey = 11;
        public BigInteger pnkey;
        public static BigInteger key;


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

        //handshake
        //serveren sender n g og ga
        //client modtager og sender bg tilbage
        //serveren tager bg og kombinere den med a

        


       /* while(true)
            {
                byte[] buffer;

        string message = "";
        byte[] beskeden;

        if 
            }*/

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

        public byte[] CreatePublicKey(string privkey, string genkey, string norkey) 
        {
            BigInteger pkey = new BigInteger(Int32.Parse(privkey)); // private key
            BigInteger gkey = new BigInteger(Int32.Parse(genkey)); // i anden
            BigInteger nkey = new BigInteger(Int32.Parse(norkey)); // MOD

            BigInteger mykey = BigInteger.ModPow(pkey, gkey, nkey);
            //Console.WriteLine('hej');
            return mykey.ToByteArray(true);
        }

        public BigInteger allkey(int privatekey3, int meutralkey, int samekey) 
        {
            BigInteger pkey = new BigInteger(privatekey3);
            BigInteger mkey = new BigInteger(meutralkey);
            BigInteger skey = new BigInteger(samekey);

            return key = BigInteger.ModPow(mkey, (int)privatekey3, skey);
        }

       /* public byte[] CreatePublicKey(string privatekey2, string generalkey) 
        {
            BigInteger nkey = new BigInteger(privatekey2);
            int nkey = Int32.Parse(privatekey2);
            int gkey = Int32.Parse(generalkey);
           // pnkey = nkey * gkey;
            return BitConverter.GetBytes(nkey * gkey);
        }
        
        public string allkey(int publickey, int ngkey) 
        {
            return (ngkey * publickey).ToString();
        }
       */
    }
}
