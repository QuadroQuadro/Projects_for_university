using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;


namespace ServerUDP
{
    static class StringHelper
    {
        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
    }
    class Program
    {
        private static IPAddress remoteIPAddress;
        private static int remotePort;
        private static int localPort;
        static Socket listeningSocket;
        private static IPAddress localIPAddress;
        static void Main(string[] args)
        {
            localIPAddress = IPAddress.Parse("192.168.1.64");
            Console.WriteLine("СерверUDP.Укажите локальный порт");
            localPort = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Укажите удаленный порт");
            remotePort = Convert.ToInt16(Console.ReadLine());
            Console.WriteLine("Введите удаленный IP");
            remoteIPAddress = IPAddress.Parse(Console.ReadLine());
            try
            {

                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                Task listeningTask = new Task(Listen);
                listeningTask.Start();

                while (true)
                {

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

                    
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);

                    do
                    {
                        bytes = listeningSocket.ReceiveFrom(data, ref remoteIp);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (listeningSocket.Available > 0);
                    
                    IPEndPoint remoteFullIp = remoteIp as IPEndPoint;

                   
                    Console.WriteLine("{0}:{1} - {2}", remoteFullIp.Address.ToString(),
                                                    remoteFullIp.Port, builder.ToString());
                    string messageReverse = StringHelper.ReverseString(builder.ToString());
                    byte[] dataReverse = Encoding.Unicode.GetBytes(messageReverse);
                    Console.WriteLine("Сообщение обработано и отправлено: " + messageReverse);
                    EndPoint remotePoint = new IPEndPoint(remoteIPAddress, remotePort);
                    listeningSocket.SendTo(dataReverse, remotePoint);
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
