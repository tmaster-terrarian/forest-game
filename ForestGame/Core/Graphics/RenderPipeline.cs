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

    private static Effect _testEffect;
    private static EffectParameter _param;
    private static EffectParameter _param2;

    private static int _resolutionScale = 4;

    public static void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        _cube = ContentLoader.Load<SimpleModel>("cube.obj")!;
        _cube.Transform = new() {
            Position = Vector3.One * -0.5f,
        };

        _cube2 = ContentLoader.Load<SimpleModel>("teapot.obj")!;
        // _cube2.Transform = Matrix.CreateTranslation(Vector3.One * -0.5f) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Left * 0.5f);
        // _cube2.Transform = Transform.CreateFromMatrix(Matrix.CreateRotationY(MathHelper.PiOver4) * Matrix.CreateTranslation(0.5f, 0, 1));
        _cube2.Transform = new() {
            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4),
            Position = new(0, -0.5f, 0),
        };

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

        _testEffect = Game1.ContentManager.Load<Effect>("fx/depth");
        _param = _testEffect.Parameters["WorldViewProjection"];
        _param2 = _testEffect.Parameters["WorldMatrix"];
    }

    public static void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.SetRenderTarget(_rt);
        GraphicsDevice.Clear(Color.Black);

        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

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

        // _cube2.Transform.Rotation = Quaternion.CreateFromAxisAngle(
        //     Vector3.Up,
        //     time / 60 * MathHelper.Pi
        // );

        _testEffect.Parameters["ViewDir"].SetValue(-Vector3.Normalize(new Vector3(
            2.5f * MathF.Cos(time * 0.01f),
            2.5f * MathF.Sin(time * 0.01f),
            2.5f * MathF.Sin(time * 0.01f)
        )));
        _testEffect.Parameters["Shininess"]?.SetValue(0.2f);
        _testEffect.Parameters["SpecularIntensity"]?.SetValue(0.5f);
        _testEffect.Parameters["Metallic"]?.SetValue(0.5f);

        foreach (EffectPass pass in _testEffect.CurrentTechnique.Passes)
        {
            // _param.SetValue(_cube.Transform * ViewMatrix * ProjectionMatrix);
            // _param2.SetValue(_cube.Transform);
            // _testEffect.Parameters["InverseWorldMatrix"]?.SetValue(Matrix.Invert(_cube.Transform));
            // pass.Apply();
            // _cube.Draw(GraphicsDevice);

            _param.SetValue(_cube2.Transform * ViewMatrix * ProjectionMatrix);
            _param2.SetValue(_cube2.Transform);
            _testEffect.Parameters["InverseWorldMatrix"]?.SetValue(Matrix.Invert(_cube2.Transform));
            pass.Apply();
            _cube2.Draw(GraphicsDevice);
        }

        CustomDraw.DrawGizmo(GraphicsDevice, Vector3.Zero);

        CustomDraw.DrawGrid(GraphicsDevice, 16, 1, Matrix.CreateTranslation(new(-8, -8, 0)) * Matrix.CreateRotationX(MathHelper.PiOver2));

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

    static Vector3 ApplyRotationOnly(Vector3 v, Matrix matrix)
    {
        matrix.Decompose(out _, out Quaternion rotationQuat, out _);
        return Vector3.Transform(v, rotationQuat);
    }
}
