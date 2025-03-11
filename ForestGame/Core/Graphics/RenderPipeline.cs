using System.Runtime.CompilerServices;
using Arch.Core;
using Arch.Core.Extensions;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public static class RenderPipeline
{
    public enum EffectPass
    {
        BasicDiffuse,
        Lit,
    }
    public enum RenderPass
    {
        World,
        Screen,
    }

    public static Matrix WorldMatrix { get; set; } = Matrix.CreateWorld(Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
    public static Matrix ViewMatrix { get; set; }
    public static Matrix ProjectionMatrix { get; set; }

    public static Texture2D WhiteTexture { get; private set; }

    public static GraphicsDevice GraphicsDevice { get; set; }
    public static GameWindow Window { get; private set; }
    public static SpriteBatch SpriteBatch { get; private set; }

    public static Camera Camera { get; set; }

    private static ObjModel _cube;
    private static GltfModel _gltfCube;
    private static Effect _effect;

    private static RenderTarget2D _rt;
    private static RenderTarget2D _rtUi;
    private static RenderTarget2D _rtSky;
    private static RenderTarget2D[] _renderTargets;
    public static RenderTarget2D[] RenderTargets => _renderTargets;

    private static readonly VertexPositionColorTexture[] _rtDrawingPrimitive = [
        new(new Vector3(0, 0, -0.5f), Color.White, Vector2.Zero),
        new(new Vector3(0, 1, -0.5f), Color.White, Vector2.UnitY),
        new(new Vector3(1, 0, -0.5f), Color.White, Vector2.UnitX),
        new(new Vector3(1, 1, -0.5f), Color.White, Vector2.One)
    ];

    private static Effect _testEffect;

    private static Effect _screenEffect;
    private static EffectParameter _screenScreenResolution;
    private static EffectParameter _screenProjection;
    private static EffectParameter _screenTexture;

    private static Texture2D? _cursorTex;

    public static bool CursorVisible { get; set; }
    public static bool GizmosVisible { get; set; }

    private static int _resolutionScale = 4;

    public static int ResolutionScale => _resolutionScale;

    private static Texture2D _matCap;
    private static Texture2D _gltfCubeTex;

    public static Effect EffectLit => _testEffect;
    public static Effect EffectBasicDiffuse => _effect;

    private static readonly List<(Aspect aspect, Transform transform)> _toDraw = [];

    private static readonly Dictionary<string, GltfModel> _modelCache = [];

    private static readonly HashSet<string> _texturesToLoad = [];
    private static readonly Dictionary<string, Texture2D> _textureCache = [];

    private static readonly HashSet<string> _matcapTexturesToLoad = [];
    private static readonly Dictionary<string, Texture2D> _matcapTextureCache = [];

    private static SkyboxRenderer _skyboxRenderer;

    internal static void Initialize(GameWindow window, GraphicsDevice graphicsDevice)
    {
        Window = window;
        Window.ClientSizeChanged += (object? sender, EventArgs e) => {
            OnWindowResize(Window.ClientBounds);
        };
        Window.AllowUserResizing = true;

        GraphicsDevice = graphicsDevice;
        WhiteTexture = new(graphicsDevice, 1, 1);
        WhiteTexture.SetData([Color.White]);
    }

    internal static void LoadContent()
    {
        SpriteBatch = new SpriteBatch(GraphicsDevice);

        _gltfCube = ContentLoader.Load<GltfModel>("models/fucking-teapot.glb")!;
        // _gltfCube = ContentLoader.Load<GltfModel>("models/sphere.glb")!;
        // _gltfCube.Transform = new() {
        //     Rotation = Quaternion.CreateFromYawPitchRoll(MathF.PI, 0, 0),
        // };

        _gltfCubeTex = ContentLoader.Load<Texture2D>("textures/checkerboard.png")!;

        _cube = ContentLoader.Load<ObjModel>("cube.obj")!;
        _cube.Transform.Origin = -Vector3.One * 0.5f;
        _cube.Transform.Position = new(-3.5f, 0.5f, -3.5f);

        _matCap = ContentLoader.Load<Texture2D>("matcaps/Matcap_Metal_04.jpeg")!;
        // _matCap = ContentLoader.Load<Texture2D>("matcaps/Matcap_Metal_02.png")!;
        // _matCap = ContentLoader.Load<Texture2D>("matcaps/Matcap_Metal_03.png")!;

        _renderTargets = [
            _rt = new(GraphicsDevice, 240, 135, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8),
            _rtUi = new(GraphicsDevice, 240, 135, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8),
            _rtSky = new(GraphicsDevice, 240, 135, false, SurfaceFormat.Color, DepthFormat.Depth24Stencil8),
        ];

        Camera = new()
        {
            Transform = new()
            {
                Position = new Vector3(0, 0, 0),
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
        _testEffect.Parameters["SkyColor"]?.SetValue(Color.White.ToVector3());
        _testEffect.Parameters["SkyTex"]?.SetValue(ContentLoader.Load<Texture2D>("textures/skybox_test_texture.png"));

        _effect = ContentLoader.Load<Effect>("fx/default")!;
        _effect.Parameters["MainTex"]?.SetValue(WhiteTexture);
        _effect.Parameters["LightIntensity"]?.SetValue(1);

        _screenEffect = ContentLoader.Load<Effect>("fx/sprite/screen")!;
        _screenScreenResolution = _screenEffect.Parameters["ScreenResolution"];
        _screenProjection = _screenEffect.Parameters["ProjectionMatrix"];
        _screenTexture = _screenEffect.Parameters["SpriteTexture"];

        _skyboxRenderer = new SkyboxRenderer("textures/skybox_test_texture.png", GraphicsDevice);

        OnWindowResize(Window.ClientBounds);
    }

    public static void Submit((Aspect aspect, Transform transform) tuple)
    {
        _toDraw.Add(tuple);
    }

    internal static void LoadTextures()
    {
        GraphicsDevice.Reset();
        foreach(var path in _texturesToLoad)
            _textureCache[path] = ContentLoader.Load<Texture2D>(path) ?? WhiteTexture;
        _texturesToLoad.Clear();
        foreach(var path in _matcapTexturesToLoad)
            _matcapTextureCache[path] = ContentLoader.Load<Texture2D>(path) ?? WhiteTexture;
        _matcapTexturesToLoad.Clear();
    }

    public static void Draw()
    {
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        GraphicsDevice.RasterizerState = new()
        {
            CullMode = CullMode.CullClockwiseFace
        };

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.Opaque;

        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(95),
            GraphicsDevice.Viewport.AspectRatio,
            0.01f, 25.0f
        );

        GraphicsDevice.SetRenderTarget(_rtSky);

        ViewMatrix = Matrix.CreateLookAt(
            Vector3.Zero,
            Camera.Transform.Matrix.Forward,
            Vector3.UnitY
        );

        _skyboxRenderer.Draw(Vector3.Zero, Quaternion.Identity);

        ViewMatrix = Matrix.CreateLookAt(
            Camera.Transform.WorldPosition,
            Camera.Transform.WorldPosition + Camera.Transform.Matrix.Forward,
            Vector3.UnitY
        );

        GraphicsDevice.SetRenderTarget(_rt);
        GraphicsDevice.Clear(Color.Transparent);

        _cube.Transform.Rotation = Quaternion.CreateFromAxisAngle(
            Vector3.UnitY,
            Time.Elapsed / 30f * MathHelper.Pi
        );

        _effect.Parameters["ViewMatrix"]?.SetValue(ViewMatrix);
        _effect.Parameters["ProjectionMatrix"]?.SetValue(ProjectionMatrix);
        _testEffect.Parameters["ViewMatrix"]?.SetValue(ViewMatrix);
        _testEffect.Parameters["ProjectionMatrix"]?.SetValue(ProjectionMatrix);

        Camera.Draw(GraphicsDevice);

        _cube.Draw(GraphicsDevice, Matrix.Identity, _effect);

        // _gltfCube.Transform.Rotation *= Quaternion.CreateFromAxisAngle(Vector3.UnitZ, 0.01f);
        // _gltfCube.Draw(GraphicsDevice, Matrix.Identity, _testEffect);

        DrawPass(_testEffect, EffectPass.Lit, RenderPass.World);
        DrawPass(_effect, EffectPass.BasicDiffuse, RenderPass.World);

        GraphicsUtil.DrawGrid(GraphicsDevice, 16, 1, Color.White * 0.1f, Matrix.CreateTranslation(new(-8, -8, 0)) * Matrix.CreateRotationX(MathHelper.PiOver2));

        GraphicsDevice.SetRenderTarget(_rtUi);
        GraphicsDevice.Clear(Color.Transparent);

        EcsManager.world.Query(new QueryDescription().WithAll<Components.Actor, Transform>(),
            (Entity entity, ref Components.Actor actor, ref Transform transform) => {
                GraphicsUtil.DrawBoundingBox(
                    GraphicsDevice,
                    actor.Collider.BoundingBox(transform.Scale),
                    Color.Orange * 0.95f,
                    actor.Velocity.Length() > 0.1f
                );
            }
        );

        if(GizmosVisible)
        {
            ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                MathHelper.ToRadians(30),
                GraphicsDevice.Viewport.AspectRatio,
                0.01f, 300.0f
            );

            GraphicsUtil.DrawGizmo(
                GraphicsDevice,
                Vector3.Zero,
                Matrix.CreateScale(0.02f)
                * Matrix.CreateTranslation(Camera.Transform.WorldPosition + Camera.Transform.Matrix.Forward)
            );
        }

        DrawPass(_testEffect, EffectPass.Lit, RenderPass.Screen);
        DrawPass(_effect, EffectPass.BasicDiffuse, RenderPass.Screen);

        if(_cursorTex is not null && CursorVisible)
        {
            SpriteBatch.Begin(samplerState: SamplerState.PointClamp);
            Vector2 cursorPos = new(Input.MousePosition.X / _resolutionScale, Input.MousePosition.Y / _resolutionScale);
            SpriteBatch.Draw(
                _cursorTex,
                cursorPos,
                Color.White
            );
            SpriteBatch.End();
        }

        GraphicsDevice.Reset();
        GraphicsDevice.BlendState = BlendState.AlphaBlend;

        _toDraw.Clear();

        DrawRt(_rtSky);

        // SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _screenEffect);
        // {
        //     _screenEffect.Parameters["ScreenResolution"]?.SetValue(new Vector2(_rt.Width, _rt.Height));
        //     SpriteBatch.Draw(_rt, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _resolutionScale, SpriteEffects.None, 0);
        // }
        // SpriteBatch.End();
        DrawRt(_rt);

        // SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _screenEffect);
        // {
        //     _screenEffect.Parameters["ScreenResolution"]?.SetValue(new Vector2(_rtUi.Width, _rtUi.Height));
        //     SpriteBatch.Draw(_rtUi, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _resolutionScale, SpriteEffects.None, 0);
        // }
        // SpriteBatch.End();
        DrawRt(_rtUi);
    }

    private static void DrawRt(RenderTarget2D rt)
    {
        // GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
        // GraphicsDevice.DepthStencilState = DepthStencilState.None;

        // foreach(var pass in _screenEffect.CurrentTechnique.Passes)
        // {
        //     pass.Apply();
        //     GraphicsDevice.DrawUserPrimitives(
        //         PrimitiveType.TriangleStrip,
        //         _rtDrawingPrimitive,
        //         0, 2,
        //         VertexPositionColorTexture.VertexDeclaration
        //     );
        // }

        SpriteBatch.Begin(samplerState: SamplerState.PointClamp, effect: _screenEffect);
        {
            _screenScreenResolution?.SetValue(new Vector2(_rtUi.Width, _rtUi.Height));
            SpriteBatch.Draw(rt, Vector2.Zero, null, Color.White, 0, Vector2.Zero, _resolutionScale, SpriteEffects.None, 0);
        }
        SpriteBatch.End();
    }

    private static void DrawPass(Effect effect, EffectPass pass, RenderPass renderPass)
    {
        var pWorldMatrix = effect.Parameters["WorldMatrix"];
        var pViewMatrix = effect.Parameters["ViewMatrix"];
        var pProjectionMatrix = effect.Parameters["ProjectionMatrix"];
        var pInverseWorldMatrix = effect.Parameters["InverseWorldMatrix"];
        var pInverseViewMatrix = effect.Parameters["InverseViewMatrix"];
        var pViewDir = effect.Parameters["ViewDir"];
        var pWorldSpaceCameraPos = effect.Parameters["WorldSpaceCameraPos"];
        var pScreenResolution = effect.Parameters["ScreenResolution"];
        var pMainTex = effect.Parameters["MainTex"];
        var pMatcapTex = effect.Parameters["MatcapTex"];
        var pMatcapIntensity = effect.Parameters["MatcapIntensity"];
        var pMatcapPower = effect.Parameters["MatcapPower"];

        var query = _toDraw.FindAll(d => d.aspect.EffectPass == pass && d.aspect.RenderPass == renderPass);
        foreach(var (aspect, transform) in query)
        {
            if(aspect.ModelPath is null)
                continue;

            pWorldMatrix?.SetValue(transform);
            pViewMatrix?.SetValue(ViewMatrix);
            pProjectionMatrix?.SetValue(ProjectionMatrix);
            pInverseWorldMatrix?.SetValue(Matrix.Invert(transform));
            pInverseViewMatrix?.SetValue(Matrix.Invert(ViewMatrix));
            pViewDir?.SetValue(Camera.Forward);
            pWorldSpaceCameraPos?.SetValue(Camera.Transform.WorldPosition);

            Vector2 vertexSnapRes = Vector2.Floor(Window.ClientBounds.Size.ToVector2() / ResolutionScale / 2);
            pScreenResolution?.SetValue(vertexSnapRes);

            if(aspect.Material.MainTexturePath is not null)
            {
                if(!_textureCache.TryGetValue(aspect.Material.MainTexturePath, out var tex))
                {
                    _texturesToLoad.Add(aspect.Material.MainTexturePath);
                    pMainTex?.SetValue(WhiteTexture);
                }
                else
                    pMainTex?.SetValue(tex);
            }
            else
                pMainTex?.SetValue(WhiteTexture);

            if(aspect.Material.MatcapOptions is not null)
            {
                if(aspect.Material.MatcapOptions.TexturePath is not null)
                {
                    if(!_matcapTextureCache.TryGetValue(aspect.Material.MatcapOptions.TexturePath, out var matcap))
                    {
                        _matcapTexturesToLoad.Add(aspect.Material.MatcapOptions.TexturePath);
                        pMatcapTex?.SetValue(WhiteTexture);
                    }
                    else
                        pMatcapTex?.SetValue(matcap);
                }
                else
                    pMatcapTex?.SetValue(WhiteTexture);

                pMatcapIntensity?.SetValue(aspect.Material.MatcapOptions.Intensity);
                pMatcapPower?.SetValue(aspect.Material.MatcapOptions.Power);
            }
            else
            {
                pMatcapTex?.SetValue(WhiteTexture);
                pMatcapIntensity?.SetValue(0);
            }

            if(aspect.ModelPath.EndsWith(".obj"))
                continue;

            if(!_modelCache.TryGetValue(aspect.ModelPath, out var model))
            {
                model = ContentLoader.Load<GltfModel>(aspect.ModelPath);
                _modelCache.Add(aspect.ModelPath, model!);
            }

            if(model is null)
                continue;

            model.Draw(GraphicsDevice, transform, effect);
        }
    }

    public static void OnWindowResize(Rectangle windowBounds)
    {
        RebuildRt(out _rt, windowBounds);
        RebuildRt(out _rtUi, windowBounds);
        RebuildRt(out _rtSky, windowBounds);

        // for whatever reason, the snap vector must be divided by 2 to make the effect visible
        // but it works, so don't touch this!!!!
        Vector2 vertexSnapRes = Vector2.Floor(windowBounds.Size.ToVector2() / _resolutionScale / 2);

        _effect.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);
        _testEffect.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);
    }

    private static void RebuildRt(out RenderTarget2D rt, Rectangle windowBounds)
    {
        rt = new(
            GraphicsDevice,
            windowBounds.Width / _resolutionScale,
            windowBounds.Height / _resolutionScale,
            false,
            SurfaceFormat.Color,
            DepthFormat.Depth24Stencil8
        );
    }
}
