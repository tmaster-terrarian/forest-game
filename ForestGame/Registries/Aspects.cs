using ForestGame.Core;
using ForestGame.Core.Graphics;

namespace ForestGame.Registries;

public static class Aspects
{
    public const string Teapot = "teapot";

    public static void Initialize()
    {
        Registry.Register<Aspect>(Teapot, new() {
            ModelPath = "models/fucking-teapot.glb",
            Material = {
                MatcapOptions = new() {
                    TexturePath = "matcaps/Matcap_Metal_04.jpeg",
                    Intensity = 1,
                    Power = 2,
                },
            },
            EffectPass = RenderPipeline.EffectPass.Lit,
            RenderPass = RenderPipeline.RenderPass.World
        });
    }
}
