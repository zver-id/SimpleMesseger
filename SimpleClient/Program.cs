using MessengerLibrary;

namespace SimpleClient;

public class Program
{
    static void Main()
    {
        Client client = new Client();
        client.ConnectToServer("127.0.0.1", 5000);

        Console.Write("Введите имя: ");
        string name = Console.ReadLine();
        User user1 = new User(name);

        client.SendObj(user1); //отправляем пользователя на сервер
        client.ReadMessages();

        while (true)
        {
            Chat currentChat = client.chats.Chats.Find(x => x.Name == "General");

            client.PrintMessages(currentChat);
            string text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text)) continue;

            Message message = new Message(text, user1);
            message.Chat = currentChat;
            client.SendObj(message);

            client.ReadMessages();
        }
    }
}