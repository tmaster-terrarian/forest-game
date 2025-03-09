namespace ForestGame.Core.Graphics;

public class Aspect
{
    public Material Material { get; set; } = new();
    public string ModelPath { get; set; }
    public RenderPipeline.EffectPass EffectPass { get; set; }
    public RenderPipeline.RenderPass RenderPass { get; set; }
}
