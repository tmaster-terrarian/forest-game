using ForestGame.Core.Graphics;

namespace ForestGame.Core;

public static class Registry
{
    private static readonly HashSet<Type> _types = [];

    internal static void Initialize()
    {
        Create<Aspect>();
        Registries.Aspects.Initialize();

        Create<Prototype>();
        Registries.Prototypes.Initialize();

        Create<IComponentSystem>();
        Registries.ComponentSystems.Initialize();
    }

    public static void Create<T>() where T : class
    {
        if(!_types.Add(typeof(T)))
            throw new InvalidOperationException($"tried to create a registry that already exists (type: {typeof(T).FullName})");
    }

    public static T Register<T>(string key, T value) where T : class
    {
        ArgumentNullException.ThrowIfNull(key, nameof(key));
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        if(!_types.Contains(typeof(T)))
            throw new InvalidOperationException($"tried register an entry in a registry that does not exist (type: {typeof(T).FullName}, key: {key})");

        if(!Registry<T>.Add(key, value))
            throw new InvalidOperationException($"tried register an entry that was already registered (key: {key})");

        return value;
    }
}

public static class Registry<T> where T : class
{
    private static readonly Dictionary<string, T> _values = [];

    private static T[] _registered;

    public static IReadOnlyList<T> Registered => _registered.AsReadOnly();

    internal static bool Add(string key, T value)
    {
        if(_values.TryAdd(key, value))
        {
            _registered = [.._values.Values];
            return true;
        }
        return false;
    }

    internal static bool Remove(string key)
    {
        if(_values.Remove(key))
        {
            _registered = [.._values.Values];
            return true;
        }
        return false;
    }

    public static T Get(string key)
    {
        return _values[key];
    }

    public static string? GetKey(T entry)
    {
        return _values.ContainsValue(entry) ? _values.FirstOrDefault(kv => kv.Value == entry).Key : null;
    }
}
