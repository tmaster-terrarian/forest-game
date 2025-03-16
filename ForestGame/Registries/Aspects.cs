using ForestGame.Core;
using ForestGame.Core.Graphics;

namespace ForestGame.Registries;

public static class Aspects
{
    public const string Teapot = "teapot";
    public const string StreetLamp = "street_lamp";
    public const string BoundingBox = "bounding_box";
    public const string Icosphere = "sphere";
    public const string Ufo = "ufo";

    public static void Initialize()
    {
        Registry.Register<Aspect>(Teapot, new ModelAspect() {
            ModelPath = "models/fucking-teapot.glb",
            Material = {
                MatcapOptions = new() {
                    TexturePath = "matcaps/mario_small.jpg",
                    Intensity = 0.35f,
                    Power = 1f,
                },
                VertexColorIntensity = 1,
            },
            EffectPass = RenderPipeline.EffectPass.MatcapOnly,
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

        Registry.Register<Aspect>(Icosphere, new ModelAspect {
            ModelPath = "models/icosphere/icosphere.glb",
            Material = {
                MatcapOptions = new() {
                    TexturePath = "matcaps/glossy_below.png",
                    Intensity = 1,
                    Power = 2,
                },
                SurfaceOptions = new() {
                    Shininess = 1,
                    Metallic = 0.5f,
                },
            },
            EffectPass = RenderPipeline.EffectPass.MatcapOnly,
            RenderPass = RenderPipeline.RenderPass.World,
        });

        Registry.Register<Aspect>(Ufo, new ModelAspect() {
            ModelPath = "models/ufo/ufo.glb",
            Material = {
                VertexColorIntensity = 1,
                MatcapOptions = new() {
                    TexturePath = "matcaps/Matcap_Metal_04.jpeg",
                    Intensity = 1,
                    Power = 2,
                    BlendMode = Material.BlendModes.Multiply,
                },
            },
            EffectPass = RenderPipeline.EffectPass.Lit,
            RenderPass = RenderPipeline.RenderPass.World
        });
    }
}
