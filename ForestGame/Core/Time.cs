using Microsoft.Xna.Framework;

namespace ForestGame.Core;

public static class Time
{
    internal static GameTime _gameTime;

    public static float TimeScale { get; set; } = 1;

    public static float UnscaledElapsed => (float)_gameTime.TotalGameTime.TotalSeconds;

    public static float Elapsed => UnscaledElapsed * TimeScale;

    public static float UnscaledDelta => (float)_gameTime.ElapsedGameTime.TotalSeconds;

    public static float Delta => UnscaledDelta * TimeScale;

    internal static void Update(GameTime gameTime)
    {
        _gameTime = gameTime;
    }
}
