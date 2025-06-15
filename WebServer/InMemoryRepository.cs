using MessengerLibrary;

namespace WebServer;

public class InMemoryRepository
{
    public List<User> Users { get; } = new List<User>();
    public List<Message> Messages { get; } = new List<Message>();
    public List<Chat> Chats { get; } = new List<Chat>();
}