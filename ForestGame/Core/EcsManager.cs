using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Core.Components;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

public static class EcsManager
{
    private static bool _initialized;

    private static readonly QueryDescription _velocityQuery = new QueryDescription().WithAll<Velocity, Transform>();

    public static World world { get; private set; }

    internal static void Update()
    {
        foreach(var system in Registry<IComponentSystem>.Registered)
        {
            system.Update();
        }
    }

    internal static void GetDrawables(GraphicsDevice graphicsDevice)
    {
        foreach(var system in Registry<IComponentSystem>.Registered)
        {
            if(system is IDrawableComponentSystem drawableSystem)
                drawableSystem.GetDrawables(graphicsDevice, RenderPipeline.Submit);
        }
    }

    internal static void Start()
    {
        if(_initialized) return;
        _initialized = true;

        world = World.Create();

        // world.SubscribeComponentAdded((in Entity entity, ref Matcapped _) =>
        // {
        //     entity.AddOrGet(ContentLoader.Load<Effect>("fx/depth"));
        // });
        // world.SubscribeComponentAdded((in Entity entity, ref IRequiresEffect _) =>
        // {
        //     entity.AddOrGet(ContentLoader.Load<Effect>("fx/default"));
        // });
    }

    internal static void Cleanup()
    {
        world.Dispose();
    }
}
