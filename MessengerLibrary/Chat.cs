using System.Collections.Generic;

namespace MessengerLibrary;
/// <summary>
/// Чат.
/// </summary>
public class Chat : Entity
{
    /// <summary>
    /// Имя чата 
    /// </summary>
    public virtual string Name { get; set; }
    
    /// <summary>
    /// Список сообщений в чате 
    /// </summary>
    public virtual List<Message> Messages { get; set; }
    
    /// <summary>
    /// Список пользователей в чате
    /// </summary>
    public virtual List<User> User { get; set; }
}