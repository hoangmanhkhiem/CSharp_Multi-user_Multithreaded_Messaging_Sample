using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server {
    class Program
    {
        private static List<TcpClient> clients = new List<TcpClient>();
        
        static void Main(string[] args) {
            TcpListener server = null;
            try {
                server = new TcpListener(IPAddress.Any, 8888);
                server.Start();
                Console.WriteLine("Server started!");

                while (true) {
                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Client connected!");

                    Thread clientThread = new Thread(() => HandleClient(client));
                    clientThread.Start();
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
            finally {
                server?.Stop();
            }
        }
        static void HandleClient(TcpClient client) {
            clients.Add(client);
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            try {
                string welcomeMessage = "Chao mung ban den voi server";
                byte[] welcomeMessageBytes = Encoding.ASCII.GetBytes(welcomeMessage);
                stream.Write(welcomeMessageBytes, 0, welcomeMessageBytes.Length);

                while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0) {
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                    BroadcastMessage(message, client);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
            finally {
                client.Close();
            }
        }

        static void BroadcastMessage(string message, TcpClient sendingClient) {
            int idx = clients.IndexOf(sendingClient);
            foreach (TcpClient client in clients) {
                if (client != sendingClient) {
                    NetworkStream stream = client.GetStream();
                    byte[] buffer = Encoding.ASCII.GetBytes(message);
                    buffer = Encoding.ASCII.GetBytes("Client " + idx + ": " + message);
                    stream.Write(buffer, 0, buffer.Length);
                    Console.WriteLine("Client " + idx + ": " + message);
                    stream.Flush();
                }
            }
        }
    }
}
