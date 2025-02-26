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

    internal static void Update(GameTime gameTime)
    {
        world.Query(_velocityQuery, (Entity entity, ref Velocity vel, ref Transform transform) =>
        {
            transform.Position += vel.Vector * gameTime.Delta();
        });
    }

    internal static void Draw(GraphicsDevice graphicsDevice, GameTime gameTime)
    {
        void drawModelType<T>() where T : class, IModel
        {
            world.Query(new QueryDescription().WithAll<Model<T>, Transform, Effect>(), (Entity entity, ref Model<T> drawer, ref Transform transform, ref Effect effect) =>
            {
                effect.Parameters["WorldMatrix"]?.SetValue(transform);
                effect.Parameters["ViewMatrix"]?.SetValue(RenderPipeline.ViewMatrix);
                effect.Parameters["ProjectionMatrix"]?.SetValue(RenderPipeline.ProjectionMatrix);
                effect.Parameters["InverseWorldMatrix"]?.SetValue(Matrix.Invert(transform));
                effect.Parameters["InverseViewMatrix"]?.SetValue(Matrix.Invert(RenderPipeline.ViewMatrix));
                effect.Parameters["ViewDir"]?.SetValue(RenderPipeline.Camera.Forward);
                effect.Parameters["WorldSpaceCameraPos"]?.SetValue(RenderPipeline.Camera.Transform.Position);

                Vector2 vertexSnapRes = Vector2.Floor(RenderPipeline.Window.ClientBounds.Size.ToVector2() / RenderPipeline.ResolutionScale / 2);
                effect.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);

                if(entity.TryGet<Textured>(out var tex))
                    effect.Parameters["MainTex"]?.SetValue(tex);

                if(entity.TryGet<Matcapped>(out var matcap))
                {
                    effect.Parameters["MatcapTex"]?.SetValue(matcap);
                    effect.Parameters["MatcapIntensity"]?.SetValue(matcap.MatcapIntensity);
                    effect.Parameters["MatcapPower"]?.SetValue(matcap.MatcapPower);
                }
                else
                {
                    effect.Parameters["MatcapTex"]?.SetValue(RenderPipeline.WhiteTexture);
                    effect.Parameters["MatcapIntensity"]?.SetValue(0);
                }

                drawer.Draw(entity, gameTime, graphicsDevice, transform, effect);
            });
        }

        drawModelType<GltfModel>();
        drawModelType<ObjModel>();
    }

    internal static void Start()
    {
        world = World.Create();

        if(_initialized) return;
        _initialized = true;

        world.SubscribeComponentAdded((in Entity entity, ref IRequiresTransform _) =>
        {
            entity.AddOrGet(Transform.Identity);
        });
        world.SubscribeComponentAdded((in Entity entity, ref Matcapped _) =>
        {
            entity.AddOrGet(ContentLoader.Load<Effect>("fx/depth"));
        });
        world.SubscribeComponentAdded((in Entity entity, ref IRequiresEffect _) =>
        {
            entity.AddOrGet(ContentLoader.Load<Effect>("fx/default"));
        });
    }

    internal static void Cleanup()
    {
        world.Dispose();
    }
}
