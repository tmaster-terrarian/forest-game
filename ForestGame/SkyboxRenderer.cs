using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame;

public class SkyboxRenderer(string imagePath, GraphicsDevice graphicsDevice)
{
    private Texture2D _texture = ContentLoader.Load<Texture2D>(imagePath)!;
    private BasicEffect _basicEffect = new BasicEffect(graphicsDevice);
    private GltfModel _unitSphere = ContentLoader.Load<GltfModel>("models/unit_sphere.glb")!;

    public void Draw(Vector3 position, Quaternion rotation)
    {
        _basicEffect.Texture = _texture;
        _basicEffect.TextureEnabled = true;
        _basicEffect.World = Matrix.CreateFromQuaternion(rotation) * Matrix.CreateTranslation(position);
        _basicEffect.View = RenderPipeline.ViewMatrix;
        _basicEffect.Projection = RenderPipeline.ProjectionMatrix;

        _unitSphere.Draw(graphicsDevice, _basicEffect.World, _basicEffect);
    }
}
