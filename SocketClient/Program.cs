using System.Net.Sockets;
using System.Net;
using System.Text;
using System;
using System.Threading.Tasks;

namespace SocketClient
{
  class Program
  {
    enum Method { HEAD, GET, POST }

    static async Task Main(string[] args)
    {
      await HEAD_MetodAsync("localhost", 80);

      await GET_MetodAsync("localhost", 80);

      await POST_MetodAsync("localhost", 80);

      QueryMetodWeb("www.google.com", 80, Method.POST);

    }

    static async Task HEAD_MetodAsync(string pServer, int pPort)
    {
      // Указываем адрес и порт сервера
      IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(pServer);
      IPAddress ipAddress = ipHostInfo.AddressList[0];
      IPEndPoint ipEndPoint = new(ipAddress, pPort);

      try
      {
        // Создание сокета и подключение к серверу
        using (var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        {
          await socket.ConnectAsync(ipEndPoint);
          while (true)
          {
            // Формирование HEAD-запроса
            string request = "HEAD / HTTP/1.0\r\n" +
                             "Host: " + ipEndPoint.AddressFamily + "\r\n" +
                             "\r\n";

            // Преобразование запроса в байты
            byte[] requestBytes = Encoding.UTF8.GetBytes(request);

            // Отправка запроса серверу
            _ = await socket.SendAsync(requestBytes, SocketFlags.None);
            Console.WriteLine($"Socket client sent message: \"{request}\"");

            // Получение и вывод ответа от сервера
            byte[] responseBytes = new byte[8192];
            var bytesRead = await socket.ReceiveAsync(responseBytes, SocketFlags.None);
            string response = Encoding.UTF8.GetString(responseBytes, 0, bytesRead);

            Console.WriteLine($"Socket client received message from server: \"{response}\"");
            break;
          }
          Console.ReadLine();
        }
      }
      catch (Exception ex)
      {
        // Обработка ошибки
        Console.WriteLine(ex.Message);
      }
    }

    static async Task GET_MetodAsync(string pServer, int pPort)
    {
      // Указываем адрес и порт сервера
      string serverAddress = pServer;
      int port = pPort;

      // Указываем адрес и порт сервера
      IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(pServer);
      IPAddress ipAddress = ipHostInfo.AddressList[0];
      IPEndPoint ipEndPoint = new(ipAddress, pPort);

      try
      {
        // Создание сокета и подключение к серверу
        using (var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        {
          //socket.Connect(serverAddress, port);
          await socket.ConnectAsync(ipEndPoint);

          // Формирование GET-запроса
          string request = "GET / HTTP/1.0\r\n" +
                           "Host: " + serverAddress + "\r\n" +
                           "\r\n";

          // Преобразование запроса в байты
          byte[] requestBytes = Encoding.ASCII.GetBytes(request);

          // Отправка запроса серверу
          //socket.Send(requestBytes);
          _ = await socket.SendAsync(requestBytes, SocketFlags.None);
          Console.WriteLine($"Socket client sent GET...");

          // Получение и вывод ответа от сервера
          byte[] responseBytes = new byte[8192];
          //int bytesRead = socket.Receive(responseBytes);
          var bytesRead = await socket.ReceiveAsync(responseBytes, SocketFlags.None);
          string response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);

          Console.WriteLine($"Socket client received message from server...");
          Console.WriteLine(response);
          Console.WriteLine("Press any key...");
          Console.ReadLine();
        }
      }
      catch (Exception ex)
      {
        // Обработка ошибки
        Console.WriteLine(ex.Message);
      }
    }

    static async Task POST_MetodAsync(string pServer, int pPort)
    {
      // Указываем адрес и порт сервера
      string serverAddress = pServer;
      int port = pPort;

      // Указываем адрес и порт сервера
      IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync(pServer);
      IPAddress ipAddress = ipHostInfo.AddressList[0];
      IPEndPoint ipEndPoint = new(ipAddress, pPort);

      try
      {
        // Создание сокета и подключение к серверу
        using (var socket = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        {
          //socket.Connect(serverAddress, port);
          await socket.ConnectAsync(ipEndPoint);

          // Формирование POST-запроса
          string postData = "param1=value1&param2=value2";
          string request = "POST /todos HTTP/1.0\r\n" +
                           "Host: " + serverAddress + "\r\n" +
                           "Content-Type: application/x-www-form-urlencoded\r\n" +
                           "Content-Length: " + postData.Length + "\r\n" +
                           "\r\n" + postData;

          // Преобразование запроса в байты
          byte[] requestBytes = Encoding.ASCII.GetBytes(request);

          // Отправка запроса серверу
          //socket.Send(requestBytes);
          _ = await socket.SendAsync(requestBytes, SocketFlags.None);
          Console.WriteLine($"Socket client sent POST...");

          // Получение и вывод ответа от сервера
          byte[] responseBytes = new byte[8192];
          //int bytesRead = socket.Receive(responseBytes);
          var bytesRead = await socket.ReceiveAsync(responseBytes, SocketFlags.None);
          string response = Encoding.ASCII.GetString(responseBytes, 0, bytesRead);

          Console.WriteLine($"Socket client received message from server...");
          Console.WriteLine(response);
          Console.WriteLine("Press any key...");
          Console.ReadLine();
        }
      }
      catch (Exception ex)
      {
        // Обработка ошибки
        Console.WriteLine(ex.Message);
      }
    }

    static void QueryMetodWeb(string pServer, int pPort, Method method)
    {
      // Устанавливаем адрес и порт сервера Google
      string serverAddress = pServer;
      int port = pPort;

      // Создаем TCP-сокет
      using (var clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
      {
        try
        {
          // Подключаемся к серверу Google
          clientSocket.Connect(serverAddress, port);
          string request ="";
          switch (method)
          {
            case Method.GET:
              // Формируем GET-запрос
              request = "GET / HTTP/1.0\r\n";
              request += "Host: " + serverAddress + "\r\n";
              request += "Connection: close\r\n";
              request += "\r\n";
              break;
            case Method.HEAD:
              // Формирование HEAD-запроса
              request = "HEAD / HTTP/1.0\r\n" +
                        "Host: " + serverAddress + "\r\n" +
                        "\r\n";
              break;
            case Method.POST:
              string postData = "param1=value1&param2=value2";
              request = "POST /todos HTTP/1.0\r\n" +
                               "Host: " + serverAddress + "\r\n" +
                               "Content-Type: application/x-www-form-urlencoded\r\n" +
                               "Content-Length: " + postData.Length + "\r\n" +
                               "\r\n" + postData;
              break;

          }
          // Конвертируем запрос в байты
          byte[] requestBytes = Encoding.ASCII.GetBytes(request);

          // Отправляем запрос серверу
          clientSocket.Send(requestBytes);

          // Буфер для получения ответа от сервера
          byte[] buffer = new byte[4096];
          StringBuilder response = new StringBuilder();

          // Получаем ответ от сервера
          int bytesRead;
          do
          {
            bytesRead = clientSocket.Receive(buffer);
            response.Append(Encoding.ASCII.GetString(buffer, 0, bytesRead));
          } while (bytesRead > 0);

          // Выводим ответ на консоль
          Console.WriteLine(response.ToString());
          Console.ReadLine();
        }
        catch (Exception ex)
        {
          Console.WriteLine("Ошибка: " + ex.Message);
        }
      }
    }

  }
}
