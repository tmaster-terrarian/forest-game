using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public abstract class Aspect
{
    public Material Material { get; set; } = new();
    public RenderPipeline.EffectPass EffectPass { get; set; }
    public RenderPipeline.RenderPass RenderPass { get; set; }

    private readonly List<Transform> _toDraw = [];

    public virtual bool CheckValid() => true;

    public virtual void Submit(Transform transform)
    {
        _toDraw.Add(transform);
    }

    public virtual void Clear()
    {
        _toDraw.Clear();
    }

    public void Draw(GraphicsDevice graphicsDevice, EffectConfig effectConfig)
    {
        foreach(var transform in _toDraw)
        {
            effectConfig.WorldMatrix?.SetValue(transform);
            effectConfig.InverseWorldMatrix?.SetValue(Matrix.Invert(transform));
            Draw(transform, graphicsDevice, effectConfig.Effect);
        }
        Clear();
    }

    protected abstract void Draw(Transform transform, GraphicsDevice graphicsDevice, Effect effect);
}
