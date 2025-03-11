using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public abstract class Aspect
{
    public Material Material { get; set; } = new();
    public RenderPipeline.EffectPass EffectPass { get; set; }
    public RenderPipeline.RenderPass RenderPass { get; set; }

    private readonly List<Transform> _toDraw = [];

    protected static readonly Dictionary<string, GltfModel> modelCache = [];

    public virtual bool CheckValid() => true;

    public virtual void Submit(Transform transform)
    {
        _toDraw.Add(transform);
    }

    public virtual void Clear()
    {
        _toDraw.Clear();
    }

    public void Draw(GraphicsDevice graphicsDevice, Effect effect, Action<Transform> setWorldCallback)
    {
        foreach(var transform in _toDraw)
        {
            // the callback is important because there isnt a
            // consistent or easy way to do that for any given effect
            setWorldCallback(transform);
            Draw(transform, graphicsDevice, effect);
        }
        Clear();
    }

    protected abstract void Draw(Transform transform, GraphicsDevice graphicsDevice, Effect effect);
}
