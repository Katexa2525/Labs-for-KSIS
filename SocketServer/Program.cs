using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System;
using System.IO;

namespace SocketServer
{
  class Program
  {
    static async Task Main(string[] args)
    {
      // Указываем порт для прослушивания
      int port = 80;

      IPHostEntry ipHostInfo = await Dns.GetHostEntryAsync("localhost");
      IPAddress ipAddress = ipHostInfo.AddressList[0];
      IPEndPoint ipEndPoint = new(ipAddress, port);

      try
      {
        // Создание сокета и привязка к локальному адресу и порту
        using (var listener = new Socket(ipEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp))
        {
          //listener.Bind(new IPEndPoint(IPAddress.Any, port));
          listener.Bind(ipEndPoint);
          listener.Listen(10);

          Console.WriteLine("Сервер запущен и ожидает подключений...");

          while (true)
          {
            // Принимаем входящее подключение
            using (var clientSocket = listener.Accept())
            {
              // Получаем запрос от клиента
              byte[] buffer = new byte[8192];
              int bytesRead = clientSocket.Receive(buffer);
              string request = Encoding.ASCII.GetString(buffer, 0, bytesRead);

              // Выводим запрос в консоль
              Console.WriteLine("Получен запрос:\n" + request);

              // Проверяем, является ли запрос HEAD
              bool isHeadRequest = request.StartsWith("HEAD", StringComparison.OrdinalIgnoreCase);

              // Формируем ответ в зависимости от типа запроса
              string response = string.Empty;
              if (request.StartsWith("HEAD", StringComparison.OrdinalIgnoreCase))
              {
                response = "HTTP/1.0 200 OK\r\n" +
                           "Content-Length: 0\r\n" +
                           "\r\n" +
                           "<h1>Hello, HEAD!</h1>";
              }
              /////////////////////////////
              else if (request.StartsWith("GET", StringComparison.OrdinalIgnoreCase))
              {
                // Получаем запрошенный путь из запроса
                string requestedPath = GetRequestedPath(request);

                // Определяем путь к файлу на сервере
                string serverFilePath = Path.Combine(Environment.CurrentDirectory + "\\SocketServer.deps.json", requestedPath.TrimStart('/'));

                // Проверяем существование файла
                if (File.Exists(serverFilePath))
                {
                  // Читаем содержимое файла
                  string fileContent = File.ReadAllText(serverFilePath);

                  // Формируем успешный ответ
                  response = "HTTP/1.0 200 OK\r\n" +
                             "Content-Type: text/html\r\n" +
                             "Content-Length: " + fileContent.Length + "\r\n" +
                             "\r\n" + fileContent;
                }
              }
              //////////////////////////////////
              else if (request.StartsWith("POST", StringComparison.OrdinalIgnoreCase))
              {
                // Получаем тело запроса
                string requestBody = GetRequestBody(request);

                // Обрабатываем полученные данные
                response = "HTTP/1.0 200 OK\r\n" +
                           "Content-Type: text/plain\r\n" +
                           "\r\n" + "Data received successfully: " + requestBody;
              }
              else
              {
                response = "HTTP/1.0 400 Bad Request\r\n" +
                           "Content-Type: text/plain\r\n" +
                           "\r\n" +
                           "Only HEAD, GET, POST requests are supported.";
              }


              // Преобразование ответа в байты
              byte[] responseBytes = Encoding.ASCII.GetBytes(response);

              // Отправляем ответ клиенту
              clientSocket.Send(responseBytes);

              // Закрываем соединение
              clientSocket.Shutdown(SocketShutdown.Both);
            }
          }
        }
      }
      catch (Exception ex)
      {
        // Обработка ошибки
        Console.WriteLine(ex.Message);
      }
    }

    // Метод для извлечения запрошенного пути из запроса GET
    static string GetRequestedPath(string request)
    {
      int start = request.IndexOf(' ') + 1;
      int end = request.IndexOf(' ', start);
      string path = request.Substring(start, end - start);
      return Uri.UnescapeDataString(path);
    }

    // Метод для извлечения тела запроса POST
    static string GetRequestBody(string request)
    {
      int start = request.IndexOf("\r\n\r\n") + 4;
      string body = request.Substring(start);
      return body;
    }

  }
}
