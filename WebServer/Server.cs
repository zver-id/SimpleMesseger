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
                    string userMessage = ReadStream(stream, buffer);
                    int bytesRead;
                    byte[] response;

                    switch (userMessage)
                    {
                        case "LogIn":
                            SendResponse("ExpectedCredentials", stream);
                            var clientResponse =  ReadStream(stream, buffer);
                            var user = Entity.FromJson<User>(clientResponse);
                            var userInRepo = repository.Get<User>(x => x.Name == user.Name && x.Password == user.Password);
                            if (userInRepo != null)
                            {
                                var handler = new Handler(repository, user);
                                SendResponse(handler.GetChatPacket(user).ToJson(), stream);
                            }
                            else
                            {
                                SendResponse("AuthorizationFailed", stream);
                            }
                            break;
                        case "Register":
                            SendResponse("ExpectedUser", stream);
                            clientResponse =  ReadStream(stream, buffer);
                            user = Entity.FromJson<User>(clientResponse);
                            userInRepo = repository.Get<User>(x => x.Name == user.Name);
                            if (userInRepo != null)
                            {
                                SendResponse("LoginExists", stream);
                            }
                            else
                            {
                                repository.Add<User>(user);
                                SendResponse("AccountSuccessfullyAdded", stream);
                            }
                            break;
                        case "SendMessage":
                            SendResponse("ExpectedMessage", stream);
                            clientResponse = ReadStream(stream, buffer);
                            var message = Entity.FromJson<Message>(clientResponse);
                            repository.Add<Message>(message);
                            //SendResponse(handler.GetChatPacket(message.Autor).ToJson(), stream);
                        break;
                    }

                    
                    
                    
                    
                    
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

    private void SendResponse(string responseText, NetworkStream stream)
    {
        byte[]response = Encoding.UTF8.GetBytes(responseText);
        stream.Write(response, 0, response.Length);
    }

    private string ReadStream(NetworkStream stream, byte[] buffer)
    {
        int bytesRead = stream.Read(buffer, 0, buffer.Length);
        return Encoding.UTF8.GetString(buffer, 0, bytesRead);
    }


}
