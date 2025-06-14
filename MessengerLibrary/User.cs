using System.Text.Json;

namespace MessengerLibrary;

/// <summary>
/// Пользователь.
/// </summary>
public class User : Entity
{
    /// <summary>
    /// Имя пользователя
    /// </summary>
    public virtual string Name { get; set; }

    public static User? FromJson (string Json)
    {
        return JsonSerializer.Deserialize<User>(Json);
    }
}