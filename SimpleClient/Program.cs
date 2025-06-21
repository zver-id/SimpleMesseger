using MessengerLibrary;

namespace SimpleClient;

public class Program
{
    static void Main()
    {
        Client client = new Client();
        client.ConnectToServer("127.0.0.1", 5000);

        User user = client.AuthUser();
        client.ReadMessages();

        while (true)
        {
            client.PrintChats(user);
        }
    }
}