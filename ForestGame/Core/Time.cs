using Microsoft.Xna.Framework;

namespace ForestGame.Core;

public static class Time
{
    private static GameTime _time;

    public static float TimeScale { get; set; } = 1;

    public static float UnscaledElapsed => (float)_time.TotalGameTime.TotalSeconds;

    public static float Elapsed => UnscaledElapsed * TimeScale;

    public static float UnscaledDelta => (float)_time.ElapsedGameTime.TotalSeconds;

    public static float Delta => UnscaledDelta * TimeScale;

    internal static void Update(GameTime gameTime)
    {
        _time = gameTime;
    }
}
