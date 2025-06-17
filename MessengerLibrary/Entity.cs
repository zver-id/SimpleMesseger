using System.Text.Json;

namespace MessengerLibrary;

public abstract class Entity
{
    public virtual int Id { get; init; }
    public static T FromJson<T> (string Json)
    {
        return JsonSerializer.Deserialize<T>(Json);
    }
}