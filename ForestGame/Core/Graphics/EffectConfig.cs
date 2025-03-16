using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public readonly record struct EffectConfig(Effect Effect)
{
    public readonly EffectParameter? WorldMatrix { get; }          = Effect?.Parameters["WorldMatrix"];
    public readonly EffectParameter? ViewMatrix { get; }           = Effect?.Parameters["ViewMatrix"];
    public readonly EffectParameter? ProjectionMatrix { get; }     = Effect?.Parameters["ProjectionMatrix"];
    public readonly EffectParameter? InverseWorldMatrix { get; }   = Effect?.Parameters["InverseWorldMatrix"];
    public readonly EffectParameter? InverseViewMatrix { get; }    = Effect?.Parameters["InverseViewMatrix"];
    public readonly EffectParameter? ViewDir { get; }              = Effect?.Parameters["ViewDir"];
    public readonly EffectParameter? WorldSpaceCameraPos { get; }  = Effect?.Parameters["WorldSpaceCameraPos"];
    public readonly EffectParameter? ScreenResolution { get; }     = Effect?.Parameters["ScreenResolution"];
    public readonly EffectParameter? MainTex { get; }              = Effect?.Parameters["MainTex"];
    public readonly EffectParameter? VertexColorIntensity { get; } = Effect?.Parameters["VertexColorIntensity"];
    public readonly EffectParameter? VertexColorBlendMode { get; } = Effect?.Parameters["VertexColorBlendMode"];
    public readonly EffectParameter? Shininess { get; }            = Effect?.Parameters["Shininess"];
    public readonly EffectParameter? Metallic { get; }             = Effect?.Parameters["Metallic"];
    public readonly EffectParameter? MatcapTex { get; }            = Effect?.Parameters["MatcapTex"];
    public readonly EffectParameter? MatcapIntensity { get; }      = Effect?.Parameters["MatcapIntensity"];
    public readonly EffectParameter? MatcapPower { get; }          = Effect?.Parameters["MatcapPower"];
    public readonly EffectParameter? MatcapBlendMode { get; }      = Effect?.Parameters["MatcapBlendMode"];
}
