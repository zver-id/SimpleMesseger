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
    /// <summary>
    /// Чат которому принадлежит сообщение
    /// </summary>
    public virtual Chat? Chat { get; set; }

    public Message(string? text, User? autor)
    {
        Text = text;
        Autor = autor;
    }
}