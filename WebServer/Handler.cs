using MessengerLibrary;

namespace WebServer;

public class Handler
{
    public User User { get; set; }
    private IRepository Repository { get; init; }

    public ChatPacket Handle(Message message)
    {
        var chat = Repository.Get<Chat>(x => x.Name == message.Chat.Name);
        chat.Messages.Add(message);
        var result = (ChatPacket)this.GetChatPacket(message.Autor).Clone();
        LimitMessegeCount(result, 10);
        return result;
    }

    private void LimitMessegeCount(ChatPacket packet, int count)
    {
        foreach (var chat in packet.Chats)
        {
            if (count > chat.Messages.Count)
                continue;
            chat.Messages = chat.Messages.GetRange(chat.Messages.Count - count, count);
        }
    }

    public Handler(IRepository repository, User user)
    {
        this.Repository = repository;
        this.User = user;
    }
    
    public ChatPacket GetChatPacket(User user)
    {
        var packet = new ChatPacket();
        foreach (var chat in Repository.GetAll<Chat>(x=>true))
        {
            if (chat.User.Contains(user))
            {
                packet.Chats.Add(chat);
            };
        }
        return packet;
    }

    public void AddUserToChat(User user, Chat chat)
    {
        
    }
    
}