using ForestGame.Core;
using ForestGame.Core.Graphics;
using ForestGame.Core.Serialization;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame;

public static class ContentLoader
{
    private static readonly string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data");

    private static readonly List<string> _missingPaths = [];

    private static ContentManager _shaderLoader = null!;
    private static GraphicsDevice _graphicsDevice = null!;

    internal static void Initialize(ContentManager shaderLoader, GraphicsDevice graphicsDevice)
    {
        if(_shaderLoader is not null)
            throw new InvalidOperationException("attempted to initialize ContentLoader after it has already been initialized");

        _shaderLoader = shaderLoader;
        _graphicsDevice = graphicsDevice;
    }

    public static T? Load<T>(string path) where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string filePath = Path.Combine(_dataPath, path);
        if(Check<T,Effect>())
        {
            filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _shaderLoader.RootDirectory, path);
        }

        lock(_missingPaths)
            if(_missingPaths.Contains(path))
                return default;

        try
        {
            var ret = LoadInternal<T>(filePath);
            if(ret is default(T))
                lock(_missingPaths)
                    _missingPaths.Add(path);
            return ret;
        }
        catch(Exception e)
        {
            Console.Error.WriteLine($"data file {path} could not be loaded: {e}");

            lock(_missingPaths)
                _missingPaths.Add(path);

            return default;
        }
    }

    private static T? LoadInternal<T>(string path) where T : class
    {
        if(!File.Exists(path) && !Check<T,SpriteEffect>() && !Check<T,Effect>())
            throw new FileNotFoundException($"file does not exist or could not be found");

        if(Check<T,Effect>())
        {
            lock(_shaderLoader)
                return _shaderLoader.Load<T>(Path.ChangeExtension(path, null));
        }
        else if (Check<T,Texture2D>())
        {
            lock(_graphicsDevice)
                return Texture2D.FromFile(_graphicsDevice, path) as T;
        }
        else if(Check<T,GltfModel>())
        {
            return GltfModel.Load(path) as T;
        }
        else if(Check<T,ObjModel>())
        {
            return ObjModel.Load(path) as T;
        }
        else if(Check<T,Stage>())
        {
            return StageSerializer.Load(path) as T;
        }

        throw new Exception($"the target type {typeof(T).FullName} is not loadable from a file");
    }

    private static bool Check<T, K>() where T : class where K : class
        => typeof(T).IsAssignableTo(typeof(K));
}
