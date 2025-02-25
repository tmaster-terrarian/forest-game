using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Components;

public interface ITextureComponent
{
    public Texture2D Texture { get; }
}

public record struct Textured(Texture2D Texture) : ITextureComponent
{
    public static implicit operator Texture2D(Textured component)
    {
        return component.Texture;
    }
}

public record struct Matcapped(Texture2D Texture, float MatcapIntensity, float MatcapPower) : ITextureComponent
{
    public static implicit operator Texture2D(Matcapped component)
    {
        return component.Texture;
    }
}
