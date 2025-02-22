using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public static class RenderPipeline
{
    public static Matrix ViewMatrix { get; set; }
    public static Matrix ProjectionMatrix { get; set; }

    public static GraphicsDevice GraphicsDevice { get; set; }
    public static SpriteBatch SpriteBatch { get; private set; }

    private static SimpleModel _cube;
    private static SimpleModel _cube2;
    private static BasicEffect _effect;
    private static RenderTarget2D _rt;

    private static int _resolutionScale = 4;

    public static void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        _cube = ContentLoader.Load<SimpleModel>("cube.obj")!;
        _cube.Transform = Matrix.CreateTranslation(Vector3.One * -0.5f);

        _cube2 = ContentLoader.Load<SimpleModel>("teapot.obj")!;
        _cube2.Transform = Matrix.CreateTranslation(Vector3.One * -0.5f) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Left * 0.5f);

        _rt = new(GraphicsDevice, 240, 135, false, SurfaceFormat.Color, DepthFormat.Depth16);

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(95),
            GraphicsDevice.Viewport.AspectRatio,
            0.01f, 300.0f
        );

        _effect = new(GraphicsDevice)
        {
            // primitive color
            AmbientLightColor = new Vector3(0.1f, 0.1f, 0.1f),
            DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f),
            SpecularColor = new Vector3(0.25f, 0.25f, 0.25f),
            SpecularPower = 5.0f,
            Alpha = 1.0f,

            // The following MUST be enabled if you want to color your vertices
            VertexColorEnabled = true
        };
    }

    public static void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.SetRenderTarget(_rt);
        GraphicsDevice.Clear(Color.Black);

        var time = (float)gameTime.TotalGameTime.TotalSeconds * 60;
        ViewMatrix = Matrix.CreateLookAt(
            new Vector3(
                2.5f * MathF.Cos(time * 0.01f),
                2.5f * MathF.Sin(time * 0.01f),
                2.5f * MathF.Sin(time * 0.01f)
            ),
            Vector3.Zero,
            Vector3.Up
        );

        _effect.View = ViewMatrix;
        _effect.Projection = ProjectionMatrix;

        GraphicsDevice.RasterizerState = new()
        {
            CullMode = CullMode.CullClockwiseFace
        };

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.Opaque;

        // foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        // {
        //     _effect.World = _cube.Transform;
        //     pass.Apply();

        //     _cube.Draw(GraphicsDevice);

        //     _effect.World = _cube2.Transform;
        //     pass.Apply();

        //     _cube2.Draw(GraphicsDevice);
        // }

        CustomDraw.DrawGizmo(GraphicsDevice, Vector3.Zero);

        CustomDraw.DrawGrid(GraphicsDevice, 16, 1, Matrix.CreateTranslation(Vector3.One * -0.5f) * Matrix.CreateTranslation(new(-8, -8, 0)));

        GraphicsDevice.Reset();

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        {
            SpriteBatch.Draw(_rt, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _resolutionScale, SpriteEffects.None, 0);
        }
        SpriteBatch.End();
    }

    public static void OnWindowResize(GameWindow window)
    {
        _rt = new(
            GraphicsDevice,
            window.ClientBounds.Width / _resolutionScale,
            window.ClientBounds.Height / _resolutionScale,
            false,
            SurfaceFormat.Color,
            DepthFormat.Depth16
        );

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(95),
            GraphicsDevice.Viewport.AspectRatio,
            0.01f, 300.0f
        );
    }
}
