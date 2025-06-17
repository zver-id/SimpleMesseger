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

    public User(string name)
    { 
        this.Name = name; 
    }
    public override bool Equals(object? obj)
    {
        if (obj is User user)
        {
            return this.Name == user.Name;
        }
        return false;
    }
}