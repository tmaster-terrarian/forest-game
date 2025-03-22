using ForestGame.Core;
using ForestGame.Core.Graphics;
using ForestGame.Renderers;

namespace ForestGame.Registries;

public static class Renderers
{
    public const string Pather = "pather";

    public static void Initialize()
    {
        Registry.Register<Renderer>(Pather, new PatherRenderer {
            EffectPass = RenderPipeline.EffectPass.VertSnapOnly,
            RenderPass = RenderPipeline.RenderPass.World,
        });
    }
}
