using System;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Linq;

namespace ServerTCP
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
        static void Main(string[] args)
        {
            
            IPHostEntry ipHost = Dns.GetHostEntry("192.168.1.64");
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, 11000);

            
            Socket sListener = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                sListener.Bind(ipEndPoint);
                sListener.Listen(10);

               
                while (true)
                {
                    Console.WriteLine("Ожидаем соединение через порт {0}", ipEndPoint);

                    
                    Socket handler = sListener.Accept();
                    string data = null;
                    
                    byte[] bytes = new byte[1024];
                    int bytesRec = handler.Receive(bytes);

                    data += Encoding.UTF8.GetString(bytes, 0, bytesRec);
                    
                    Console.Write("Полученный текст: " + data + "\n\n");
                   
        string reply = "Перевернутая строка выглядит так: " + StringHelper.ReverseString(data);
                    byte[] msg = Encoding.UTF8.GetBytes(reply);
                    handler.Send(msg);

                    if (data.IndexOf("<TheEnd>") > -1)
                    {
                        Console.WriteLine("Сервер завершил соединение с клиентом.");
                        break;
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.ReadLine();
            }
        }
    }
}
