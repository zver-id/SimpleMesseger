using MessengerLibrary;

namespace WebServer;

public class Handler
{
    public User User { get; set; }
    private InMemoryRepository Repository { get; init; }

    public ChatPacket Handle(Message message)
    {
        var chat = message.Chat;
        chat.Messages.Add(message);
        return this.GetChatPacket(message.Autor);
    }

    public Handler(InMemoryRepository repository, User user)
    {
        this.Repository = repository;
        this.User = user;
    }
    
    public ChatPacket GetChatPacket(User user)
    {
        var packet = new ChatPacket();
        foreach (var chat in Repository.Chats)
        {
            if (chat.User.Contains(user))
            {
                packet.Chats.Add(chat);
            };
        }
        return packet;
    }
    
}