using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public class ModelAspect : Aspect
{
    public string ModelPath { get; set; }

    public override bool CheckValid()
    {
        return ModelPath is not null && !ModelPath.EndsWith(".obj");
    }

    protected override void Draw(Transform transform, GraphicsDevice graphicsDevice, Effect effect)
    {
        var model = modelCache.AddOrGet(ModelPath, () => ContentLoader.Load<GltfModel>(ModelPath));
        if(model is null)
            return;

        model.Draw(graphicsDevice, transform, effect);
    }
}
