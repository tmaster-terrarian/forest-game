using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public static class RenderPipeline
{
    public enum Pass
    {
        WorldBasicDiffuse,
        WorldLit,
        ScreenBasicDiffuse,
        ScreenLit,
    }

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
    private static Effect _effect;
    private static RenderTarget2D _rt;
    private static RenderTarget2D _rtUi;

    private static Effect _testEffect;

    private static Effect _screenEffect;

    private static Texture2D? _cursorTex;

    private static int _resolutionScale = 4;

    public static int ResolutionScale => _resolutionScale;

    private static Texture2D _matCap;
    private static Texture2D _gltfCubeTex;

    public static Effect EffectLit => _testEffect;
    public static Effect EffectBasicDiffuse => _effect;

    internal static readonly HashSet<(Pass Pass, Action<Components.IDrawModel> DrawAction)> toDraw = [];

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
        _cube.Transform.Origin = -Vector3.One * 0.5f;
        _cube.Transform.Position = new(-3.5f, 0.5f, -3.5f);

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

        _testEffect = ContentLoader.Load<Effect>("fx/depth")!;
        _testEffect.Parameters["MatcapTex"]?.SetValue(_matCap);
        _testEffect.Parameters["MainTex"]?.SetValue(WhiteTexture);
        _testEffect.Parameters["MatcapIntensity"]?.SetValue(1f);
        _testEffect.Parameters["MatcapPower"]?.SetValue(2f);
        _testEffect.Parameters["Shininess"]?.SetValue(0.5f);
        _testEffect.Parameters["Metallic"]?.SetValue(1f);

        for(int i = 0; i < 5; i++)
        EcsManager.world.Create(
            new Components.Model<GltfModel>("models/fucking-teapot.glb"),
            new Transform {
                Position = -Vector3.UnitX * 3 * i
            },
            _testEffect,
            new Components.Matcapped(_matCap, 1, 2)
        );

        _effect = ContentLoader.Load<Effect>("fx/default")!;
        _effect.Parameters["MainTex"]?.SetValue(WhiteTexture);
        _effect.Parameters["LightIntensity"]?.SetValue(1);

        _screenEffect = ContentLoader.Load<Effect>("fx/screen")!;
    }

    public static void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.SetRenderTarget(_rt);
        GraphicsDevice.Clear(Color.Black);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

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

        GraphicsDevice.RasterizerState = new()
        {
            CullMode = CullMode.CullClockwiseFace
        };

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.Opaque;

        _cube.Transform.Rotation = Quaternion.CreateFromAxisAngle(
            Vector3.UnitY,
            (float)gameTime.TotalGameTime.TotalSeconds / 30f * MathHelper.Pi
        );

        _effect.Parameters["ViewMatrix"]?.SetValue(ViewMatrix);
        _effect.Parameters["ProjectionMatrix"]?.SetValue(ProjectionMatrix);
        _testEffect.Parameters["ViewMatrix"]?.SetValue(ViewMatrix);
        _testEffect.Parameters["ProjectionMatrix"]?.SetValue(ProjectionMatrix);

        Camera.Draw(GraphicsDevice);

        _cube.Draw(GraphicsDevice, Matrix.Identity, _effect);

        // _gltfCube.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.01f);
        // _gltfCube.Draw(GraphicsDevice, Matrix.Identity, _testEffect);

        EcsManager.Draw(GraphicsDevice, gameTime);

        GraphicsUtil.DrawGrid(GraphicsDevice, 16, 1, Matrix.CreateTranslation(new(-8, -8, 0)) * Matrix.CreateRotationX(MathHelper.PiOver2));

        SpriteBatch.End();

        GraphicsDevice.Reset();

        GraphicsDevice.SetRenderTarget(_rtUi);
        GraphicsDevice.Clear(Color.Transparent);

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp);

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

        if(_cursorTex is not null)
        {
            Vector2 cursorPos = new(Input.MousePosition.X / _resolutionScale, Input.MousePosition.Y / _resolutionScale);
            SpriteBatch.Draw(
                _cursorTex,
                cursorPos,
                Color.White
            );
        }

        SpriteBatch.End();

        GraphicsDevice.Reset();

        toDraw.Clear();

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

        // for whatever reason, the snap vector must be divided by 2 to make the effect visible
        // but it works, so don't touch this!!!!
        Vector2 vertexSnapRes = Vector2.Floor(Window.ClientBounds.Size.ToVector2() / _resolutionScale / 2);

        _effect.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);
        _testEffect.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);
    }
}
