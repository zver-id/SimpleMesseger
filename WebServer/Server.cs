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
    private InMemoryRepository repository;

    public Server(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        repository = new InMemoryRepository();
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
                var user = User.FromJson(userMessage);
                if (!repository.Users.Contains(user))
                {
                    repository.Users.Add(user);
                    var generalCart = repository.Chats.Find(x => x.Name == "General");
                    generalCart.User.Add(user);
                    byte[] bytes = Encoding.UTF8.GetBytes(GetChatPacket(user).ToJson());
                }
                else
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(GetChatPacket(user).ToJson());
                }
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

    private ChatPacket GetChatPacket(User user)
    {
        var packet = new ChatPacket();
        foreach (var chat in repository.Chats)
        {
            if (chat.User.Contains(user))
            {
                packet.Chats.Add(chat);
            };
        }
        return packet;
    }
}
