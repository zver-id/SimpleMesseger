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
                    Handler handler = null;
                    User user;
                    while (handler == null)
                    {
                        switch (userMessage)
                        {
                            case ClientServerDialog.LogInMessage:
                                SendResponse(ClientServerDialog.ExpectedCredentialsMessage, stream);
                                var clientResponse = ReadStream(stream, buffer);
                                user = Entity.FromJson<User>(clientResponse);
                                var userInRepo =
                                    repository.Get<User>(x => x.Name == user.Name && x.Password == user.Password);
                                if (userInRepo != null)
                                {
                                    handler = new Handler(repository, userInRepo);
                                    var generalCart = repository.Get<Chat>(x => x.Name == "General");
                                    generalCart.User.Add(userInRepo);
                                    SendResponse(ClientServerDialog.UserFoundMessage, stream); 
                                    SendResponse(handler.GetChatPacket(userInRepo).ToJson(), stream);
                                }
                                else
                                {
                                    SendResponse(ClientServerDialog.AuthorizationFailedMessage, stream);
                                    user = null;
                                }

                                break;
                            case ClientServerDialog.RegisterMessage:
                                SendResponse(ClientServerDialog.ExpectedUserMessage, stream);
                                clientResponse = ReadStream(stream, buffer);
                                user = Entity.FromJson<User>(clientResponse);
                                userInRepo = repository.Get<User>(x => x.Name == user.Name);
                                if (userInRepo != null)
                                {
                                    SendResponse(ClientServerDialog.LoginExistsMessage, stream);
                                    user = null;
                                }
                                else
                                {
                                    repository.Add<User>(userInRepo);
                                    handler = new Handler(repository, userInRepo);
                                    var generalCart = repository.Get<Chat>(x => x.Name == "General");
                                    generalCart.User.Add(userInRepo);
                                    SendResponse(ClientServerDialog.UserAddedMessage, stream); 
                                    SendResponse(handler.GetChatPacket(userInRepo).ToJson(), stream);
                                }

                                break;
                            default:
                                SendResponse($"{ClientServerDialog.UnexpectedMessage}" +
                                             $"expected  {ClientServerDialog.LogInMessage}" +
                                             $" or {ClientServerDialog.RegisterMessage}", stream);
                                break; 
                        }
                    }
                    
                    while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        userMessage = ReadStream(stream, buffer);
                        switch (userMessage)
                        {
                            case ClientServerDialog.CreateChatMessage:
                                SendResponse(ClientServerDialog.ExpectedChatMessage, stream);
                                var clientResponse = ReadStream(stream, buffer);
                                var newChat = Entity.FromJson<Chat>(clientResponse);
                                var chatInRepo = repository.Get<Chat>(x => x.Name == newChat.Name);
                                if (chatInRepo != null)
                                {
                                    SendResponse(ClientServerDialog.ChatExistsMessage, stream);
                                }
                                else
                                {
                                    repository.Add<Chat>(newChat);
                                    SendResponse(ClientServerDialog.ChatAddedMessage, stream);
                                    SendResponse(handler.GetChatPacket(handler.User).ToJson(), stream);
                                }
                                break;
                            case ClientServerDialog.AddUserToChatMessage:
                                SendResponse(ClientServerDialog.ExpectedChatMessage, stream);
                                clientResponse = ReadStream(stream, buffer);
                                var chat = Entity.FromJson<Chat>(clientResponse);
                                chatInRepo = repository.Get<Chat>(x => x.Name == chat.Name);
                                if (chatInRepo != null)
                                {
                                    SendResponse(ClientServerDialog.ExpectedUserMessage, stream);
                                    clientResponse = ReadStream(stream, buffer);
                                    var newUser = Entity.FromJson<User>(clientResponse);
                                    var userInRepo = repository.Get<User>(x => x.Name == newUser.Name);
                                    if (userInRepo != null)
                                    {
                                        if (chatInRepo.User.Contains(userInRepo))
                                            SendResponse(ClientServerDialog.AlreadyInChatMessage, stream);
                                        else
                                            chatInRepo.User.Add(userInRepo);
                                    }
                                    else
                                    {
                                        SendResponse(ClientServerDialog.UserNotFoundMessage, stream);
                                    }
                                }
                                break;
                            default:
                                clientResponse = ReadStream(stream, buffer);
                                var message = Entity.FromJson<Message>(clientResponse);
                                repository.Add<Message>(message);
                                var messageHandler = new Handler(repository, message.Autor);
                                SendResponse(messageHandler.GetChatPacket(message.Autor).ToJson(), stream);
                                break;
                        }
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
