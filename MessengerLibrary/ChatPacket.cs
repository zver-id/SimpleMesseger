using System;
using System.Collections.Generic;
using System.Text.Json;

namespace MessengerLibrary;

/// <summary>
/// Пакет для передачи от сервера клиенту
/// </summary>
public class ChatPacket : ICloneable
{
    /// <summary>
    /// Список чатов для передачи
    /// </summary>
    public virtual List<Chat>? Chats { get; set; } = new List<Chat>();


    public string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    public ChatPacket(User user)
    {
        
    }

    public ChatPacket()
    {
        
    }

    public object Clone()
    {
        return new ChatPacket
        {
            Chats = new List<Chat>(this.Chats)
        };
    }
}