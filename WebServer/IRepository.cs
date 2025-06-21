using MessengerLibrary;

namespace WebServer;

public interface IRepository
{
    public void Add<T> (T entity);
    public T Get<T> (Predicate<T> match);
    public List<T> GetAll<T> (Predicate<T> match);
    public bool Exists<T>(Predicate<T> match);
}