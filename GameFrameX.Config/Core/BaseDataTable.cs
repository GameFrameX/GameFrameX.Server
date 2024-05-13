

using System.Text.Json;

namespace GameFrameX.Config.Core;

public abstract class BaseDataTable<T> : IDataTable<T>
{
    protected readonly SortedDictionary<long, T> LongDataMaps = new SortedDictionary<long, T>();
    protected readonly SortedDictionary<string, T> StringDataMaps = new SortedDictionary<string, T>();

    protected readonly List<T> DataList = new List<T>();
    protected readonly System.Func<System.Threading.Tasks.Task<JsonElement>> _loadFunc;

    public BaseDataTable(Func<Task<JsonElement>> loadFunc)
    {
        _loadFunc = loadFunc;
    }

    public BaseDataTable()
    {
        
    }

    public abstract Task LoadAsync();

    public T Get(int id)
    {
        LongDataMaps.TryGetValue(id, out T value);
        return value;
    }

    public T Get(string id)
    {
        StringDataMaps.TryGetValue(id, out T value);
        return value;
    }

    public T this[int id] => Get(id);

    public T this[string id] => Get(id);

    public int Count => Math.Max(LongDataMaps.Count, StringDataMaps.Count);

    public T FirstOrDefault => DataList.FirstOrDefault();

    public T LastOrDefault => DataList.LastOrDefault();

    public T[] All => DataList.ToArray();

    public T[] ToArray()
    {
        return DataList.ToArray();
    }

    public T Find(Func<T, bool> func)
    {
        return DataList.FirstOrDefault(func);
    }

    public T[] FindList(Func<T, bool> func)
    {
        return DataList.Where(func).ToArray();
    }
}