using System.Collections.Generic;
using System.Text.Json;

namespace MessengerLibrary;

/// <summary>
/// Пакет для передачи от сервера клиенту
/// </summary>
public class ChatPacket
{
    /// <summary>
    /// Список чатов для передачи
    /// </summary>
    public virtual List<Chat>? Chats { get; set; } = new List<Chat>();

    public virtual Message? Message { get; set; }

    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public ChatPacket(List<Chat>? chats, Message? message)
    {
        this.Chats = chats; 
        this.Message = message;
    }

    public ChatPacket(User user)
    {
        
    }

    public ChatPacket()
    {
        
    }
}