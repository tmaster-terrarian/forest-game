using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core.Graphics;

public static class RenderPipeline
{
    public enum EffectPass
    {
        BasicDiffuse,
        Lit,
        MatcapOnly,
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
    private static Effect _diffuse;

    private static RenderTarget2D _rt;
    private static RenderTarget2D _rtUi;
    private static RenderTarget2D _rtSky;
    private static RenderTarget2D[] _renderTargets;
    public static RenderTarget2D[] RenderTargets => _renderTargets;

    private static Effect _lit;

    private static Effect _matCapOnly;

    private static Effect _screenEffect;
    private static EffectParameter _screenScreenResolution;

    private static Effect _vertsnapEffect;

    private static Texture2D? _cursorTex;

    public static bool CursorVisible { get; set; }
    public static bool GizmosVisible { get; set; }

    private static int _resolutionScale = 4;

    public static int ResolutionScale => _resolutionScale;

    private static Texture2D _matCap;

    public static Effect EffectLit => _lit;
    public static Effect EffectBasicDiffuse => _diffuse;
    public static Effect EffectMatcapOnly => _matCapOnly;

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

        _lit = ContentLoader.Load<Effect>("fx/depth")!;
        _lit.Parameters["MatcapTex"]?.SetValue(_matCap);
        _lit.Parameters["MainTex"]?.SetValue(WhiteTexture);
        _lit.Parameters["VertexColorIntensity"]?.SetValue(1f);
        _lit.Parameters["MatcapIntensity"]?.SetValue(1f);
        _lit.Parameters["MatcapPower"]?.SetValue(2f);
        _lit.Parameters["Shininess"]?.SetValue(0.5f);
        _lit.Parameters["Metallic"]?.SetValue(1f);
        _lit.Parameters["SkyColor"]?.SetValue(Color.White.ToVector3());
        _lit.Parameters["SkyTex"]?.SetValue(ContentLoader.Load<Texture2D>("textures/skybox_test_texture.png"));

        _diffuse = ContentLoader.Load<Effect>("fx/default")!;
        _diffuse.Parameters["MainTex"]?.SetValue(WhiteTexture);

        _screenEffect = ContentLoader.Load<Effect>("fx/sprite/screen")!;
        _screenScreenResolution = _screenEffect.Parameters["ScreenResolution"];

        _skyboxRenderer = new SkyboxRenderer("textures/skybox_test_texture.png", GraphicsDevice);

        _matCapOnly = ContentLoader.Load<Effect>("fx/matcap")!;

        _vertsnapEffect = ContentLoader.Load<Effect>("fx/vertsnap")!;

        OnWindowResize(Window.ClientBounds);
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
        ProjectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(95),
            GraphicsDevice.Viewport.AspectRatio,
            0.01f, 25.0f
        );

        GraphicsDevice.Reset();
        ResetGraphicsDevice();
        GraphicsDevice.SetRenderTarget(_rtSky);

        ViewMatrix = Matrix.CreateLookAt(
            Vector3.Zero,
            Camera.Transform.Matrix.Forward,
            Vector3.UnitY
        );

        _skyboxRenderer.Draw(Vector3.Zero, Quaternion.Identity);

        GraphicsDevice.Reset();
        ResetGraphicsDevice();
        GraphicsDevice.SetRenderTarget(_rt);
        GraphicsDevice.Clear(Color.Transparent);

        ViewMatrix = Matrix.CreateLookAt(
            Camera.Transform.WorldPosition,
            Camera.Transform.WorldPosition + Camera.Transform.Matrix.Forward,
            Vector3.UnitY
        );

        _cube.Transform.Rotation = Quaternion.CreateFromAxisAngle(
            Vector3.UnitY,
            Time.Elapsed / 30f * MathHelper.Pi
        );

        _lit.Parameters["ViewMatrix"]?.SetValue(ViewMatrix);
        _lit.Parameters["InverseViewMatrix"]?.SetValue(Matrix.Invert(ViewMatrix));
        _lit.Parameters["ProjectionMatrix"]?.SetValue(ProjectionMatrix);
        _diffuse.Parameters["ViewMatrix"]?.SetValue(ViewMatrix);
        _diffuse.Parameters["InverseViewMatrix"]?.SetValue(Matrix.Invert(ViewMatrix));
        _diffuse.Parameters["ProjectionMatrix"]?.SetValue(ProjectionMatrix);

        // _diffuse.Parameters["LightIntensity"]?.SetValue(1);
        // _cube.Draw(GraphicsDevice, Matrix.Identity, _diffuse);

        DrawPass(ref _lit, EffectPass.Lit, RenderPass.World);
        DrawPass(ref _diffuse, EffectPass.BasicDiffuse, RenderPass.World);
        DrawPass(ref _matCapOnly, EffectPass.MatcapOnly, RenderPass.World);

        GraphicsUtil.DrawGrid(GraphicsDevice, 16, 1, Color.White * 0.1f, Matrix.CreateTranslation(new(-8, -8, 0)) * Matrix.CreateRotationX(MathHelper.PiOver2));

        _vertsnapEffect.Parameters["ViewMatrix"]?.SetValue(ViewMatrix);
        _vertsnapEffect.Parameters["ProjectionMatrix"]?.SetValue(ProjectionMatrix);
        GraphicsUtil.DrawText(
            SpriteBatch,
            "hi",
            new Transform {
                Position = new(3, 0, 3),
                Rotation = Quaternion.CreateFromAxisAngle(Vector3.UnitY, 0)
            },
            Color.White,
            Vector2.UnitY,
            _vertsnapEffect
        );

        GraphicsDevice.Reset();
        ResetGraphicsDevice();
        GraphicsDevice.SetRenderTarget(_rtUi);
        GraphicsDevice.Clear(Color.Transparent);

        // EcsManager.world.Query(new QueryDescription().WithAll<Components.Actor, Transform>(),
        //     (Entity entity, ref Components.Actor actor, ref Transform transform) => {
        //         GraphicsUtil.DrawBoundingBox(
        //             GraphicsDevice,
        //             actor.Collider.BoundingBox(transform.Scale),
        //             Color.Orange * 0.95f * 0.8f,
        //             Color.Yellow * 0.95f,
        //             actor.Velocity.Length()
        //         );
        //     }
        // );

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

        DrawPass(ref _lit, EffectPass.Lit, RenderPass.Screen);
        DrawPass(ref _diffuse, EffectPass.BasicDiffuse, RenderPass.Screen);
        DrawPass(ref _matCapOnly, EffectPass.MatcapOnly, RenderPass.Screen);

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
        ResetGraphicsDevice();
        GraphicsDevice.BlendState = BlendState.AlphaBlend;

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

    private static void ResetGraphicsDevice()
    {
        GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

        GraphicsDevice.RasterizerState = new()
        {
            CullMode = CullMode.CullClockwiseFace
        };

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.Opaque;
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

    private static void DrawPass(ref Effect effect, EffectPass pass, RenderPass renderPass)
    {
        EffectConfig effectConfig = new(effect);

        var aspects =
            from a in Registry<Aspect>.Registered
            where a.EffectPass == pass && a.RenderPass == renderPass
            where a.CheckValid()
            select a;

        foreach(var aspect in aspects)
        {
            effectConfig.ViewMatrix?.SetValue(ViewMatrix);
            effectConfig.ProjectionMatrix?.SetValue(ProjectionMatrix);
            effectConfig.InverseViewMatrix?.SetValue(Matrix.Invert(ViewMatrix));
            effectConfig.ViewDir?.SetValue(Camera.Forward);
            effectConfig.WorldSpaceCameraPos?.SetValue(Camera.Transform.WorldPosition);

            Vector2 vertexSnapRes = Vector2.Floor(Window.ClientBounds.Size.ToVector2() / ResolutionScale / 2);
            effectConfig.ScreenResolution?.SetValue(vertexSnapRes);

            if(aspect.Material.MainTexturePath is not null)
            {
                if(!_textureCache.TryGetValue(aspect.Material.MainTexturePath, out var tex))
                {
                    _texturesToLoad.Add(aspect.Material.MainTexturePath);
                    effectConfig.MainTex?.SetValue(WhiteTexture);
                }
                else
                    effectConfig.MainTex?.SetValue(tex);
            }
            else
                effectConfig.MainTex?.SetValue(WhiteTexture);

            effectConfig.VertexColorIntensity?.SetValue(aspect.Material.VertexColorIntensity);
            effectConfig.VertexColorBlendMode?.SetValue((int)aspect.Material.VertexColorBlendMode);

            if(aspect.Material.MatcapOptions is not null)
            {
                if(aspect.Material.MatcapOptions.TexturePath is not null)
                {
                    if(!_matcapTextureCache.TryGetValue(aspect.Material.MatcapOptions.TexturePath, out var matcap))
                    {
                        _matcapTexturesToLoad.Add(aspect.Material.MatcapOptions.TexturePath);
                        effectConfig.MatcapTex?.SetValue(WhiteTexture);
                    }
                    else
                        effectConfig.MatcapTex?.SetValue(matcap);
                }
                else
                    effectConfig.MatcapTex?.SetValue(WhiteTexture);

                effectConfig.MatcapIntensity?.SetValue(aspect.Material.MatcapOptions.Intensity);
                effectConfig.MatcapPower?.SetValue(aspect.Material.MatcapOptions.Power);
                effectConfig.MatcapBlendMode?.SetValue((int)aspect.Material.MatcapOptions.BlendMode);
            }
            else
            {
                effectConfig.MatcapTex?.SetValue(WhiteTexture);
                effectConfig.MatcapIntensity?.SetValue(0);
                effectConfig.MatcapBlendMode?.SetValue(0);
            }

            if(aspect.Material.SurfaceOptions is not null)
            {
                effectConfig.Shininess?.SetValue(aspect.Material.SurfaceOptions.Shininess);
                effectConfig.Metallic?.SetValue(aspect.Material.SurfaceOptions.Metallic);
            }
            else
            {
                effectConfig.Shininess?.SetValue(0.5f);
                effectConfig.Metallic?.SetValue(0);
            }

            aspect.Draw(GraphicsDevice, effectConfig);

            ResetGraphicsDevice();
        }

        var renderers =
            from r in Registry<Renderer>.Registered
            where r.Enabled && r.EffectPass == pass && r.RenderPass == renderPass
            select r;

        foreach(var renderer in renderers)
        {
            effectConfig.ViewMatrix?.SetValue(ViewMatrix);
            effectConfig.ProjectionMatrix?.SetValue(ProjectionMatrix);
            effectConfig.InverseViewMatrix?.SetValue(Matrix.Invert(ViewMatrix));
            effectConfig.ViewDir?.SetValue(Camera.Forward);
            effectConfig.WorldSpaceCameraPos?.SetValue(Camera.Transform.WorldPosition);

            Vector2 vertexSnapRes = Vector2.Floor(Window.ClientBounds.Size.ToVector2() / ResolutionScale / 2);
            effectConfig.ScreenResolution?.SetValue(vertexSnapRes);

            if(renderer.MainTexturePath is not null)
            {
                if(!_textureCache.TryGetValue(renderer.MainTexturePath, out var tex))
                {
                    _texturesToLoad.Add(renderer.MainTexturePath);
                    effectConfig.MainTex?.SetValue(WhiteTexture);
                }
                else
                    effectConfig.MainTex?.SetValue(tex);
            }
            else
                effectConfig.MainTex?.SetValue(WhiteTexture);

            renderer.Draw(GraphicsDevice, effectConfig);

            ResetGraphicsDevice();
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

        _diffuse.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);
        _lit.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);
        _vertsnapEffect.Parameters["ScreenResolution"]?.SetValue(vertexSnapRes);
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

    internal static void Cleanup()
    {
        _texturesToLoad.Clear();
        _textureCache.Clear();
        _matcapTexturesToLoad.Clear();
        _matcapTextureCache.Clear();
        Camera = new();
    }
}
