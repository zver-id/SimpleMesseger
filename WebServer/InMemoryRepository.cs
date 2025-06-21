using System.Reflection;
using MessengerLibrary;

namespace WebServer;

public class InMemoryRepository:IRepository
{
    private List<User> Users { get; } = new List<User>();
    private List<Message> Messages { get; } = new List<Message>();
    private List<Chat> Chats { get; set; } = new List<Chat>();
    
    public void Add<T>(T entity)
    {
        Type entityType = entity.GetType();
        var repoType = this.GetType();

        var property  = repoType.GetProperty($"{entityType.Name}s", BindingFlags.NonPublic | BindingFlags.Instance);

        if (property != null && property.PropertyType.IsGenericType &&
            property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            Type genericType = typeof(List<>);
            Type listType = genericType.MakeGenericType(entityType);
            List<T> list = (List<T>) property.GetValue(this);
            list.Add(entity);
        }
    }

    public T? Get<T>(Predicate<T> match)
    {
        var repoType = this.GetType();

        var property  = repoType.GetProperty($"{typeof(T).Name}s", BindingFlags.NonPublic | BindingFlags.Instance);

        if (property != null && property.PropertyType.IsGenericType &&
            property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            List<T> list = (List<T>) property.GetValue(this);
            return list.Find(match);
        }
        else
        {
            return default;
        }
    }

    public List<T>? GetAll<T>(Predicate<T> match)
    {
        var repoType = this.GetType();
        var property  = repoType.GetProperty($"{typeof(T).Name}s", BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.PropertyType.IsGenericType &&
            property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            List<T> list = (List<T>) property.GetValue(this);
            return list.FindAll(match);
        }
        else
        {
            return default;
        }
    }

    public bool Exists<T>(Predicate<T> match)
    {
        var repoType = this.GetType();
        var property  = repoType.GetProperty($"{typeof(T).Name}s", BindingFlags.NonPublic | BindingFlags.Instance);
        if (property != null && property.PropertyType.IsGenericType &&
            property.PropertyType.GetGenericTypeDefinition() == typeof(List<>))
        {
            List<T> list = (List<T>) property.GetValue(this);
            return list.Exists(match);
        }
        else
        {
            return false;
        }
    }

    public InMemoryRepository()
    {
        Chats.Add(new Chat("General"));
    }
}