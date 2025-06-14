using MessengerLibrary;

namespace WebServer;

public class Handler
{
    public ChatPacket Response {get; set;} 
    public User Autor {get; set;}
    public Handler(ChatPacket packet)
    {
        if (packet.Message == null)
        {
            this.Response = new ChatPacket();
        }
        else
        {
            //записать в хранилище
        }
    }
    public Handler(User user)
    {
        this.Autor = user;
    }
}