using Microsoft.Xna.Framework.Graphics;

namespace ForestGame;

public static class Global
{
    private static Func<GraphicsDevice?> _graphicsGetter = () => null;

    public static Func<GraphicsDevice?> GetGraphics => _graphicsGetter;

    internal static void SetGraphicsDelegate(Func<GraphicsDevice?> func)
    {
        _graphicsGetter = func;
    }
}
