using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core;

public class Game1 : Game
{
    private static readonly object _graphicsLock = new();

    private GraphicsDeviceManager _graphics;

    private float Frame = 0;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "shaders";

        IsMouseVisible = false;

        IsFixedTimeStep = false;
    }

    protected override void Initialize()
    {
        ContentLoader.Initialize(Content, GraphicsDevice);

        Window.ClientSizeChanged += (object? sender, EventArgs e) => {
            RenderPipeline.OnWindowResize(Window);
        };
        Window.AllowUserResizing = true;

        Locale.Configure(new() {
            RelativeDataPath = "data/lang",
            SearchRecursively = false,
        });
        Locale.CurrentLanguage = "en-us";

        EcsManager.Start();

        // StageManager.Initialize();

        base.Initialize();
    }

    protected override void LoadContent()
    {
        RenderPipeline.Window = Window;
        RenderPipeline.GraphicsDevice = GraphicsDevice;
        RenderPipeline.LoadContent();
    }

    protected override void Update(GameTime gameTime)
    {
        Input.InputDisabled = !IsActive;
        Input.RefreshKeyboardState();
        Input.RefreshMouseState();
        Input.RefreshGamePadState();
        Input.UpdateTypingInput(gameTime);

        if (Input.GetPressed(Buttons.Back) || Input.GetPressed(Keys.Escape))
            Exit();

        EcsManager.Update(gameTime);

        RenderPipeline.Camera.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        RenderPipeline.Draw(gameTime);

        base.Draw(gameTime);

        Frame = (float)gameTime.TotalGameTime.TotalSeconds * 60;
    }
}
