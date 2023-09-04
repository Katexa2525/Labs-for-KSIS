using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ConsoleApp_Time
{
  class Program
  {
    static void Main(string[] args)
    {
      GetTime("time.nist.gov", ProtocolType.Tcp, 37);
      
      GetTime("time.nist.gov", ProtocolType.Udp, 37);
    }

    static void GetTime(string timeServer, ProtocolType protocolType, int port)
    {
      //string timeServer = "time.nist.gov"; // адрес сервера времени
      //int port = 37; // стандартный порт протокола Time
      byte[] buffer;

      // создаем сокет для подключения к серверу
      Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      socket.Connect(new IPEndPoint(Dns.GetHostAddresses(timeServer)[0], port));

      if (protocolType == ProtocolType.Udp)
      {
        // отправляем запрос на сервер
        buffer = new byte[1];
        socket.Send(buffer);
      }

      // получаем данные от сервера
      buffer = new byte[4];
      socket.Receive(buffer);

      // закрываем соединение с сервером
      socket.Close();

      // преобразуем полученные данные в DateTime
      uint seconds = (uint)(buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3]);
      DateTime dateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);

      Console.WriteLine("Текущее время: " + dateTime);
      Console.WriteLine("Нажмите любую клавишу...");
      Console.ReadLine();
    }

    static void GetTimeToProtocolTcp(string timeServer, ProtocolType protocolType)
    {
      //string timeServer = "time.nist.gov"; // адрес сервера времени
      int port = 37; // стандартный порт протокола Time

      // создаем сокет для подключения к серверу
      Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
      socket.Connect(new IPEndPoint(Dns.GetHostAddresses(timeServer)[0], port));

      // получаем данные от сервера
      byte[] buffer = new byte[4];
      socket.Receive(buffer);

      // закрываем соединение с сервером
      socket.Close();

      // преобразуем полученные данные в DateTime
      uint seconds = (uint)(buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3]);
      DateTime dateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);

      Console.WriteLine("Текущее время: " + dateTime);
      Console.ReadLine();
    }

    static void GetTimeToProtocolUdp(string timeServer)
    {
      //string timeServer = "time.nist.gov"; // адрес сервера времени
      int port = 37; // стандартный порт протокола Time

      // создаем сокет для подключения к серверу
      Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
      socket.Connect(new IPEndPoint(Dns.GetHostAddresses(timeServer)[0], port));

      // отправляем запрос на сервер
      byte[] buffer = new byte[1];
      socket.Send(buffer);

      // получаем данные от сервера
      buffer = new byte[4];
      socket.Receive(buffer);

      // закрываем соединение с сервером
      socket.Close();

      // преобразуем полученные данные в DateTime
      uint seconds = (uint)(buffer[0] << 24 | buffer[1] << 16 | buffer[2] << 8 | buffer[3]);
      DateTime dateTime = new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);

      Console.WriteLine("Текущее время: " + dateTime);
      Console.ReadLine();
    }

  }
}
