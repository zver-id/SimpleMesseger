namespace MessengerLibrary;

/// <summary>
/// Сообщение.
/// </summary>
public class Message : Entity
{
    /// <summary>
    /// Текст сообщения
    /// </summary>
    public virtual string? Text { get; set; }
    
    /// <summary>
    ///  Автор сообщения
    /// </summary>
    public virtual User? Autor { get; set; }
}