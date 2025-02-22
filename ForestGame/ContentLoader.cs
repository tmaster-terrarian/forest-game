using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame;

public static class ContentLoader
{
    private static readonly string _dataPath = Path.Join(
        AppDomain.CurrentDomain.BaseDirectory,
        "data"
    );

    private static readonly List<string> _missingPaths = [];

    public static T? Load<T>(string path) where T : class
    {
        string filePath = Path.Join(_dataPath, path);

        lock(_missingPaths)
            if(_missingPaths.Contains(path))
                return default;

        // try
        // {
            return LoadInternal<T>(filePath);
        // }
        // catch(Exception e)
        // {
        //     Console.Error.WriteLine($"data file {path} could not be loaded: {e}");

        //     lock(_missingPaths)
        //         _missingPaths.Add(path);

        //     return default;
        // }
    }

    private static T? LoadInternal<T>(string path) where T : class
    {
        if(!File.Exists(path))
        {
            throw new FileNotFoundException($"file does not exist or could not be found");
        }

        if(typeof(T).IsAssignableTo(typeof(Texture2D)))
        {
            return Texture2D.FromFile(Global.GetGraphics(), path) as T;
        }
        if(typeof(T).IsAssignableTo(typeof(SimpleModel)))
        {
            return SimpleModel.Load(path) as T;
        }

        throw new Exception($"the target type {typeof(T).FullName} is not loadable from a file");
    }
}
