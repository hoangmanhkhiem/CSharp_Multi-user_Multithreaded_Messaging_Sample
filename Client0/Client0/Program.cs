using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Client {
    class Program {
        static void Main(string[] args) {
            try {
                TcpClient client = new TcpClient("localhost", 8888);
                Console.WriteLine("Da ket noi den may chu!");

                NetworkStream stream = client.GetStream();

                Thread receiveThread = new Thread(() => ReceiveData(stream));
                receiveThread.Start();

                while (true) {
                    string tinNhanGuiDi = Console.ReadLine();
                    if (tinNhanGuiDi.ToLower() == "exit") {
                        break;
                    }
                    byte[] buffer = Encoding.ASCII.GetBytes(tinNhanGuiDi);
                    stream.Write(buffer, 0, buffer.Length);
                    stream.Flush();
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Lỗi: " + ex.Message);
            }
        }

        static void ReceiveData(NetworkStream stream) {
            try {
                byte[] bufferNhan = new byte[1024];
                while (true) {
                    int soByteDaNhan = stream.Read(bufferNhan, 0, bufferNhan.Length);
                    string phanHoi = Encoding.ASCII.GetString(bufferNhan, 0, soByteDaNhan);
                    Console.WriteLine(phanHoi);
                }
            }
            catch (Exception ex) {
                Console.WriteLine("Error " + ex.Message);
            }
        }
    }
}