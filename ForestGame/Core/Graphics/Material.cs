using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class Material
{
    public string? MainTexturePath { get; set; }
    public Matcap? MatcapOptions { get; set; }
    public Surface SurfaceOptions { get; set; } = new();
    public float VertexColorIntensity { get; set; } = 1;

    public class Matcap
    {
        public required string TexturePath { get; set; }
        public float Power { get; set; } = 2;
        public float Intensity { get; set; } = 1;
    }

    public class Surface
    {
        public float Shininess { get; set; } = 0.5f;
        public float Metallic { get; set; } = 1;
    }
}
