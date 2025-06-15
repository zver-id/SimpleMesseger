using MessengerLibrary;

namespace WebServer;

public class InMemoryRepository
{
    public List<User> Users { get; } = new List<User>();
    public List<Message> Messages { get; } = new List<Message>();
    public List<Chat> Chats { get; set; } = new List<Chat>();

    public InMemoryRepository()
    {
        Chats.Add(new Chat("General"));
    }
}