using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using MessengerLibrary;

namespace WebServer;

class Server
{
    private TcpListener listener;

    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
    }

    public void Start()
    {
        listener.Start();
        Console.WriteLine("Сервер запущен");
        while (true)
        {
            var client = listener.AcceptTcpClient();
            Thread clientThread = new Thread(HandleClient);
            clientThread.Start(client);
        }
    }

    private void HandleClient(object obj)
    {
        using (var client = (TcpClient)obj)
        using (var stream = client.GetStream())
        {
            byte[] buffer = new byte[1024];
            int bytesRead;
            bytesRead = stream.Read(buffer, 0, buffer.Length);
            string userMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            if (userMessage != null)
            {
                byte[] response = Encoding.UTF8.GetBytes("OK");
                stream.Write(response, 0, response.Length);
            }

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead); 
                //Console.WriteLine("Получено сообщение: " + message);
                var handler = new Handler(User.FromJson(message));
                
                // ответ
                //byte[] response = Encoding.UTF8.GetBytes(handler.Response.ToJson());
                //stream.Write(response, 0, response.Length);
            }
        }
    }
}
