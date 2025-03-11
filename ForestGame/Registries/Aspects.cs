using ForestGame.Core;
using ForestGame.Core.Graphics;

namespace ForestGame.Registries;

public static class Aspects
{
    public const string Teapot = "teapot";
    public const string StreetLamp = "street_lamp";
    public const string BoundingBox = "bounding_box";

    public static void Initialize()
    {
        Registry.Register<Aspect>(Teapot, new ModelAspect() {
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

        Registry.Register<Aspect>(StreetLamp, new ModelAspect() {
            ModelPath = "models/street_lamp/street_lamp.glb",
            Material = {
                MainTexturePath = "models/street_lamp/base_map.png",
                MatcapOptions = new() {
                    TexturePath = "matcaps/Matcap_Metal_04.jpeg",
                    Intensity = 1,
                    Power = 2,
                },
                VertexColorIntensity = 0,
            },
            EffectPass = RenderPipeline.EffectPass.Lit,
            RenderPass = RenderPipeline.RenderPass.World
        });

        Registry.Register<Aspect>(BoundingBox, new BoundingBoxAspect() {
            RenderPass = RenderPipeline.RenderPass.Screen,
        });
    }
}
