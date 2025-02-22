using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core;

public class Game1 : Game
{
    private static readonly object _graphicsLock = new();

    private GraphicsDeviceManager _graphics;

    public static ContentManager ContentManager { get; private set; }

    private float Frame = 0;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);

        ContentManager = Content;
        Content.RootDirectory = "shaders";

        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnWindowResize;

        base.Initialize();
    }

    protected override void LoadContent()
    {
        Global.SetGraphicsDelegate(() => {
            lock(_graphicsLock)
                return this.GraphicsDevice;
        });

        RenderPipeline.GraphicsDevice = GraphicsDevice;
        RenderPipeline.LoadContent();
    }

    private void OnWindowResize(object? sender, EventArgs e)
    {
        RenderPipeline.OnWindowResize(Window);
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
        RenderPipeline.Draw(gameTime);

        base.Draw(gameTime);

        Frame = (float)gameTime.TotalGameTime.TotalSeconds * 60;
    }
}
