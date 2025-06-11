using System.Collections.Generic;

namespace MessengerLibrary;

/// <summary>
/// Пакет для передачи от сервера клиенту
/// </summary>
public class ChatPacket
{
    /// <summary>
    /// Список чатов для передачи
    /// </summary>
    public virtual List<Chat> Chats { get; set; } = new List<Chat>();
}