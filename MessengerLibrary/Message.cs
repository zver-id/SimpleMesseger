namespace MessengerLibrary;

/// <summary>
/// Сообщение.
/// </summary>
public class Message : Entity
{
    /// <summary>
    /// Текст сообщения
    /// </summary>
    public string? Text { get; set; }
    
    /// <summary>
    ///  Автор сообщения
    /// </summary>
    public User? Autor { get; set; }
}