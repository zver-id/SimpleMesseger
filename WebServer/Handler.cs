using MessengerLibrary;

namespace WebServer;

public class Handler
{
    public ChatPacket Response {get; set;} 
    public Handler(ChatPacket packet)
    {
        if (packet.Message == null)
        {
            this.Response = new ChatPacket();
        }
        else
        {
            
        }
        
    }
}