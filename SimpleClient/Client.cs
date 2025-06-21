using System;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using MessengerLibrary;

namespace SimpleClient;
public class Client
{
    private TcpClient client;
    private NetworkStream stream;
    public ChatPacket chats;

    public void ConnectToServer(string ipAddress, int port)
    {
        client = new TcpClient(ipAddress, port);
        stream = client.GetStream();
    }

    public void ReadMessages()
    {
        byte[] buffer = new byte[10485760];
        int bytesRead;
        bytesRead = stream.Read(buffer, 0, buffer.Length);

        string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        try
        {
            ChatPacket chatPacket = JsonSerializer.Deserialize<ChatPacket>(json);
            chats = chatPacket;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка при разборе сообщения" + ex.Message);
        }
    }

    public void PrintMessages(Chat currentChat)
    {
        Console.Clear();

        Console.WriteLine("Нажмите Esc, чтобы вернуться к списку чатов");

        try
        {
            if (currentChat != null)
            {
                Console.WriteLine(currentChat.Name);

                foreach (var message in currentChat.Messages)
                {
                    Console.WriteLine($"{message.Autor.Name}: {message.Text}");
                }
                Console.Write("> ");
            }
            else
            {
                Console.WriteLine("Чата не существует");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Ошибка вывода текущего чата" + ex.Message + ex.StackTrace);
        }
    }

    private string PrintMenu()
    {
        Console.WriteLine("Введите номер нужного пункта (1/2)\n" +
            "1. Войти в аккаунт.\n" +
            "2. Зарегистрироваться.\n");
        string userChoice = Console.ReadLine();
        return userChoice;
    }

    public User AuthUser()
    {
        string userChoice = PrintMenu();

        Console.Write("Имя пользователя: ");
        string name = Console.ReadLine();
        Console.Write("Пароль: ");
        string password = Console.ReadLine();

        Console.Clear();

        if (userChoice == "1")
        {
            string clientMessage = "Log in";
            SendString(clientMessage);

        }
        else if (userChoice == "2")
        {
            string clientMessage = "Sign up";
            SendString(clientMessage);
        }

        User user = new User(name); //создаем пользователя в обоих случаях?
        user.Password = password;

        SendObj(user);

        return user;
    }

    public void RunChat(User user, string chatName)
    {
        Console.Clear();

        while (true)
        {
            Chat currentChat = chats.Chats.Find(x => x.Name == chatName);

            PrintMessages(currentChat);

            string text = ReadLineWithEsc();

            if (text == null)
            {
                Console.WriteLine("Выход из чата");
                break;
            }

            if (string.IsNullOrWhiteSpace(text)) continue;

            Message message = new Message(text, user);
            message.Chat = currentChat;

            SendString("Take message");
            SendObj(message);

            ReadMessages();
        }
    }

    public void PrintChats(User user)
    {
        Console.Clear();

        foreach(var chat in chats.Chats)
        {
            Console.WriteLine(chat.Name);
        }
        Console.Write("Хотите добавить новый чат? y/n");
        string userChoice = Console.ReadLine();
        if(userChoice == "y")
        {
            AddNewChat(user);
        }
        else if(userChoice == "n")
        {
            Console.Write("Введите название нужного чата из списка: ");
            string chatName = Console.ReadLine();

            RunChat(user, chatName);
        }
    }

    public void AddNewChat(User user)
    {
        Console.Write("Введите название чата: ");
        string chatName = Console.ReadLine();

        Chat newChat = new Chat(chatName);

        newChat.User.Add(user);

        string exit = "y";
        while(exit == "y") 
        {
            User newUser = PrintUsers();
            newChat.User.Add(newUser);
            Console.Write("Хотите еще добавить пользователя в чат? y/n");
            exit = Console.ReadLine();
        }
        SendString("Add new chat");
        SendObj(newChat);

        RunChat(user, chatName);
    }

    public User PrintUsers()
    {
        Chat generalChat = chats.Chats.Find(x => x.Name == "General");
        try
        {
            foreach (var user in generalChat.User)
            {
                Console.WriteLine(user.Name);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка вывода списка пользователей" + e.Message);
        }
        Console.Write("Введите имя пользователя из списка, если хотите добавить его в чат");
        string name = Console.ReadLine();
        try
        {
            User newUser = generalChat.User.Find(x => x.Name == name);
            return newUser;
        }
        catch (Exception e)
        {
            Console.WriteLine("Выбранный пользователь не найден" + e.Message);
            return new User("");
        }

    }

    public string ReadLineWithEsc()
    {
        StringBuilder input = new StringBuilder();

        while(true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Escape)
            {
                return null;
            }
            else if(key.Key == ConsoleKey.Enter)
            {
                Console.WriteLine();
                return input.ToString();
            }
            else if(key.Key == ConsoleKey.Backspace)
            {
                input.Length--;
                Console.Write("\b \b");
            }
            else if(!char.IsControl(key.KeyChar))
            {
                input.Append(key.KeyChar);
                Console.Write(key.KeyChar);
            }
        }
    }

    public void SendObj<T>(T obj)
    {
        if (stream != null && stream.CanWrite)
        {
            string json = JsonSerializer.Serialize(obj);
            byte[] data = Encoding.UTF8.GetBytes(json);
            stream.Write(data, 0, data.Length);
        }
    }

    public void SendString(string str)
    {
        if (stream != null && stream.CanWrite)
        {
            byte[] data = Encoding.UTF8.GetBytes(str);
            stream.Write(data, 0, data.Length);
        }
    }
}

