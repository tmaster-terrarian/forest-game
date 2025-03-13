using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class ModelAspect : Aspect
{
    private static readonly Dictionary<string, GltfModel> _modelCache = [];

    public string ModelPath { get; set; }

    public override bool CheckValid()
    {
        return ModelPath is not null && !ModelPath.EndsWith(".obj");
    }

    internal static void Cleanup()
    {
        _modelCache.Clear();
    }

    protected override void Draw(Transform transform, GraphicsDevice graphicsDevice, Effect effect)
    {
        var model = _modelCache.AddOrGet(ModelPath, () => ContentLoader.Load<GltfModel>(ModelPath));
        if(model is null)
            return;

        model.Draw(graphicsDevice, transform, effect);
    }
}
