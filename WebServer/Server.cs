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
    private IRepository repository;

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
            try
            {
                var client = listener.AcceptTcpClient();
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
            catch(IOException)
            {
                Console.WriteLine("Клиент разорвал подключение");
            }
        }
    }

    private void HandleClient(object obj)
    {
        using (var client = (TcpClient)obj)
            try
            {
                using (var stream = client.GetStream())
                {
                    byte[] buffer = new byte[10485760];
                    int bytesRead;
                    bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string userMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    var user = Entity.FromJson<User>(userMessage);
                    var handler = new Handler(repository, user);
                    
                    byte[] response;
                    if (repository.Get<User>(x=> x.Name == user.Name)==null)
                    {
                        repository.Add<User>(user);
                        var generalCart = repository.Get<Chat>(x => x.Name == "General");
                        generalCart.User.Add(user);
                        Console.WriteLine(handler.GetChatPacket(user).ToJson());
                        response = Encoding.UTF8.GetBytes(handler.GetChatPacket(user).ToJson());
                    }
                    else
                    {
                        response = Encoding.UTF8.GetBytes(handler.GetChatPacket(user).ToJson());
                    }
                    stream.Write(response, 0, response.Length);
                    
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        string message = Encoding.UTF8.GetString(buffer, 0, bytesRead); 
                        var packetToSend = handler.Handle(Entity.FromJson<Message>(message));
                        response = Encoding.UTF8.GetBytes(packetToSend.ToJson());
                        stream.Write(response, 0, response.Length);
                    }
                }

            }
            catch (IOException e)
            {
                Console.WriteLine("Клиент разорвал подключение");
            }

    }


}
