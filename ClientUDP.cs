using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace ClientUDP
{
    class Program
    {
        private static IPAddress remoteIPAddress;
        private static int remotePort;
        private static int localPort;
        private static IPAddress localIPAddress;
        static Socket listeningSocket;
        static void Main(string[] args)
        {
            localIPAddress = IPAddress.Parse("192.168.1.64");
            Console.WriteLine("КлиентUDP. Укажите локальный порт");
            localPort = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Укажите удаленный порт");
            remotePort = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Введите удаленный IP");
            remoteIPAddress = IPAddress.Parse(Console.ReadLine());
            Console.WriteLine("Чтобы перевернуть строку, напишите ее и нажмите Enter:");

            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Task listeningTask = new Task(Listen);
                listeningTask.Start();


                while (true)
                {

                    string message = Console.ReadLine();
                    byte[] data = Encoding.Unicode.GetBytes(message);
                    EndPoint remotePoint = new IPEndPoint(remoteIPAddress, remotePort);
                    listeningSocket.SendTo(data, remotePoint);

                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        private static void Listen()
        {
            try
            {

                IPEndPoint localIP = new IPEndPoint(localIPAddress, localPort);
                listeningSocket.Bind(localIP);

                while (true)
                {
                    
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; 
                    byte[] data = new byte[256]; 

                    //адрес, с которого пришли данные
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                    do
                    {
                        bytes = listeningSocket.ReceiveFrom(data, ref remoteIp);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (listeningSocket.Available > 0);
                    
                    IPEndPoint remoteFullIp = remoteIp as IPEndPoint;

                    
                    Console.WriteLine("{0}:{1} - Получено обработанное сообщение: {2}", remoteFullIp.Address.ToString(),
                                                    remoteFullIp.Port, builder.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Close();
            }
        }
        
        private static void Close()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
        }
    }
}


