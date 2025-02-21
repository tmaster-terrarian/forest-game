using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core;

public class Game1 : Game
{
    private static readonly object _graphicsLock = new();

    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;

    private SimpleModel? _cube;
    private SimpleModel? _cube2;
    private BasicEffect _effect;
    private BasicEffect _effect2;
    private RenderTarget2D _rt;
    private Matrix worldMatrix, worldMatrix2, viewMatrix, projectionMatrix;

    private int Frame = 0;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "data";
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Global.SetGraphicsDelegate(() => {
            lock(_graphicsLock)
                return this.GraphicsDevice;
        });

        _spriteBatch = new SpriteBatch(GraphicsDevice);

        _cube = ContentLoader.Load<SimpleModel>("cube.obj");
        _cube2 = ContentLoader.Load<SimpleModel>("cube.obj");

        _rt = new(GraphicsDevice, 240, 135, false, SurfaceFormat.Color, DepthFormat.Depth16);

        worldMatrix = Matrix.CreateTranslation(Vector3.One * -0.5f) * Matrix.CreateScale(10);
        worldMatrix2 = Matrix.CreateTranslation(Vector3.One * -0.5f) * Matrix.CreateScale(5) * Matrix.CreateTranslation(Vector3.Left * 5);

        viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 50), Vector3.Zero, Vector3.Up);

        projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.PiOver4,
            GraphicsDevice.Viewport.AspectRatio,
            1.0f, 300.0f
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

        _effect2 = new(GraphicsDevice)
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

        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);

        GraphicsDevice.SetRenderTarget(_rt);
        GraphicsDevice.Clear(Color.Black);

        _spriteBatch.Begin();

        viewMatrix = Matrix.CreateLookAt(
            new Vector3(
                50 * MathF.Cos(Frame * 0.05f),
                50 * MathF.Sin(Frame * 0.05f),
                50 * MathF.Sin(Frame * 0.05f)
            ),
            Vector3.Zero,
            Vector3.Up
        );

        _effect.World = worldMatrix;
        _effect.View = viewMatrix;
        _effect.Projection = projectionMatrix;

        GraphicsDevice.RasterizerState = new()
        {
            CullMode = CullMode.CullClockwiseFace
        };

        GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        GraphicsDevice.BlendState = BlendState.Opaque;

        foreach (EffectPass pass in _effect.CurrentTechnique.Passes)
        {
            _effect.World = worldMatrix;
            pass.Apply();

            _cube?.Draw(GraphicsDevice);

            _effect.World = worldMatrix2;
            pass.Apply();

            _cube2?.Draw(GraphicsDevice);
        }

        _spriteBatch.End();

        GraphicsDevice.Reset();

        _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        {
            _spriteBatch.Draw(_rt, Window.ClientBounds with {X = 0, Y = 0}, Color.White);
        }
        _spriteBatch.End();

        base.Draw(gameTime);

        Frame++;
    }
}
