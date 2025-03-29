using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

public static class EcsManager
{
    private static bool _initialized;

    public static World world { get; private set; }

    internal static void Update()
    {
        foreach(var system in Registry<ISystem>.Registered)
        {
            if(Global.Editor && system is not ISystem.EditorUpdate)
                continue;

            system.Update();
        }
    }

    internal static void GetDrawables(GraphicsDevice graphicsDevice)
    {
        foreach(var system in Registry<ISystem>.Registered)
        {
            if(system is ISystem.Drawable drawableSystem)
                drawableSystem.GetDrawables(graphicsDevice);
        }
    }

    internal static void Start()
    {
        if(_initialized) return;
        _initialized = true;

        world = World.Create();
    }

    internal static void Restart()
    {
        Cleanup();
        world = World.Create();
    }

    internal static void Cleanup()
    {
        world.Dispose();
    }
}
