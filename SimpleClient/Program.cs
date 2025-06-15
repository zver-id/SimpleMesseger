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
            client.PrintMessages(0);
            string text = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(text)) continue;

            Message message = new Message(text, user1);
            client.SendObj(message);

            client.ReadMessages();
        }
    }
}