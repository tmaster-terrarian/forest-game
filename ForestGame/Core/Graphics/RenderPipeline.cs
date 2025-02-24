using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public static class RenderPipeline
{
    public static Matrix WorldMatrix { get; set; } = Matrix.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
    public static Matrix ViewMatrix { get; set; }
    public static Matrix ProjectionMatrix { get; set; }

    public static Texture2D WhiteTexture { get; private set; }

    public static GraphicsDevice GraphicsDevice { get; set; }
    public static GameWindow Window { get; set; }
    public static SpriteBatch SpriteBatch { get; private set; }

    public static Camera Camera { get; set; }

    private static ObjModel _cube;
    private static ObjModel _cube2;
    private static GltfModel _gltfCube;
    private static BasicEffect _effect;
    private static RenderTarget2D _rt;
    private static RenderTarget2D _rtUi;

    private static Effect _testEffect;
    private static EffectParameter _worldParam;
    private static EffectParameter _viewParam;
    private static EffectParameter _projectionParam;
    private static EffectParameter _inverseViewParam;

    private static Effect _screenEffect;

    private static Texture2D? _cursorTex;

    private static int _resolutionScale = 4;

    private static Texture2D _matCap;
    private static Texture2D _gltfCubeTex;

    public static void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        WhiteTexture = new(GraphicsDevice, 1, 1);
        WhiteTexture.SetData([Color.White]);

        _gltfCube = ContentLoader.Load<GltfModel>("models/fucking-teapot.glb")!;
        // _gltfCube = ContentLoader.Load<GltfModel>("models/sphere.glb")!;
        // _gltfCube.Transform = new() {
        //     Rotation = Quaternion.CreateFromYawPitchRoll(MathF.PI, 0, 0),
        // };

        _gltfCubeTex = ContentLoader.Load<Texture2D>("textures/checkerboard.png")!;

        _cube = ContentLoader.Load<ObjModel>("cube.obj")!;
        _cube.Transform = new() {
            Position = Vector3.One * -0.5f,
        };

        _cube2 = ContentLoader.Load<ObjModel>("teapot.obj")!;
        // _cube2.Transform = Matrix.CreateTranslation(Vector3.One * -0.5f) * Matrix.CreateScale(0.5f) * Matrix.CreateTranslation(Vector3.Left * 0.5f);
        // _cube2.Transform = Transform.CreateFromMatrix(Matrix.CreateRotationY(MathHelper.PiOver4) * Matrix.CreateTranslation(0.5f, 0, 1));
        _cube2.Transform = new() {
            Rotation = Quaternion.CreateFromAxisAngle(Vector3.Up, MathHelper.PiOver4),
            Position = new(0, -0.5f, 0),
        };

        _matCap = ContentLoader.Load<Texture2D>("matcaps/Matcap_Metal_04.jpeg")!;
        // _matCap = ContentLoader.Load<Texture2D>("matcaps/Matcap_Metal_02.png")!;
        // _matCap = ContentLoader.Load<Texture2D>("matcaps/Matcap_Metal_03.png")!;

        _rt = new(GraphicsDevice, 240, 135, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);
        _rtUi = new(GraphicsDevice, 240, 135, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8);

        Camera = new()
        {
            Transform = new()
            {
                Position = new Vector3(0, 1.3f, 2),
                Rotation = Quaternion.CreateFromYawPitchRoll(0, -MathHelper.PiOver4, 0)
            }
        };

        _cursorTex = ContentLoader.Load<Texture2D>("textures/cursor.png");

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

        _testEffect = ContentLoader.Load<Effect>("fx/depth")!;
        _worldParam = _testEffect.Parameters["WorldMatrix"];
        _viewParam = _testEffect.Parameters["ViewMatrix"];
        _projectionParam = _testEffect.Parameters["ProjectionMatrix"];
        _inverseViewParam = _testEffect.Parameters["InverseViewMatrix"];
        _testEffect.Parameters["MatcapTex"]?.SetValue(_matCap);
        _testEffect.Parameters["MainTex"]?.SetValue(WhiteTexture);
        _testEffect.Parameters["MatcapIntensity"]?.SetValue(1f);
        _testEffect.Parameters["MatcapPower"]?.SetValue(2f);

        _screenEffect = ContentLoader.Load<Effect>("fx/screen")!;
    }

    public static void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.SetRenderTarget(_rt);
        GraphicsDevice.Clear(Color.Black);

        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        ViewMatrix = Matrix.CreateLookAt(
            Camera.Transform.Position,
            Camera.Transform.Position + Camera.Forward,
            Vector3.Up
        );

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(95),
            GraphicsDevice.Viewport.AspectRatio,
            0.01f, 300.0f
        );

        _effect.World = WorldMatrix;
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

        _testEffect.Parameters["ViewDir"]?.SetValue(Camera.Forward);
        _testEffect.Parameters["Shininess"]?.SetValue(0.5f);
        _testEffect.Parameters["Metallic"]?.SetValue(1f);

        _viewParam?.SetValue(ViewMatrix);
        _projectionParam?.SetValue(ProjectionMatrix);
        _inverseViewParam?.SetValue(Matrix.Invert(ViewMatrix));
        _testEffect.Parameters["InverseWorldMatrix"]?.SetValue(Matrix.Invert(WorldMatrix));
        _testEffect.Parameters["WorldSpaceCameraPos"]?.SetValue(Camera.Transform.Position);

        // _gltfCube.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.01f);
        _gltfCube.Draw(GraphicsDevice, Matrix.Identity, _testEffect);

        GraphicsUtil.DrawGrid(GraphicsDevice, 16, 1, Matrix.CreateTranslation(new(-8, -8, 0)) * Matrix.CreateRotationX(MathHelper.PiOver2));

        GraphicsDevice.Reset();

        GraphicsDevice.SetRenderTarget(_rtUi);
        GraphicsDevice.Clear(Color.Transparent);

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(30),
            GraphicsDevice.Viewport.AspectRatio,
            0.01f, 300.0f
        );

        GraphicsUtil.DrawGizmo(
            GraphicsDevice,
            Vector3.Zero,
            Matrix.CreateScale(0.02f)
            * Matrix.CreateTranslation(Camera.Transform.Position + Camera.Forward)
        );

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
        {
            if(_cursorTex is not null)
            {
                SpriteBatch.Draw(
                    _cursorTex,
                    new Vector2(Input.MousePosition.X / _resolutionScale, Input.MousePosition.Y / _resolutionScale),
                    Color.White
                );
            }
        }
        SpriteBatch.End();

        GraphicsDevice.Reset();

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _screenEffect);
        {
            _screenEffect.Parameters["ScreenResolution"]?.SetValue(new Vector2(_rt.Width, _rt.Height));
            SpriteBatch.Draw(_rt, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _resolutionScale, SpriteEffects.None, 0);
        }
        SpriteBatch.End();

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _screenEffect);
        {
            _screenEffect.Parameters["ScreenResolution"]?.SetValue(new Vector2(_rtUi.Width, _rtUi.Height));
            SpriteBatch.Draw(_rtUi, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _resolutionScale, SpriteEffects.None, 0);
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
            DepthFormat.Depth24Stencil8
        );
        _rtUi = new(
            GraphicsDevice,
            window.ClientBounds.Width / _resolutionScale,
            window.ClientBounds.Height / _resolutionScale,
            false,
            SurfaceFormat.Color,
            DepthFormat.Depth24Stencil8
        );
    }
}
