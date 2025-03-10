using ForestGame.Core.Graphics;

namespace ForestGame.Core;

public static class Internals
{
    internal static Game1 _game;
    internal static bool _focusClicked = false;

    private static bool _exited = false;

    internal static void Initialize()
    {
        ContentLoader.Initialize(_game.Content, _game.GraphicsDevice);

        RenderPipeline.Initialize(_game.Window, _game.GraphicsDevice);

        Locale.Configure(new() {
            RelativeDataPath = "data/lang",
            SearchRecursively = false,
        });
        Locale.CurrentLanguage = "en-us";

        EcsManager.Start();

        Registry.Initialize();

        // StageManager.Initialize();
    }

    internal static void LoadContent()
    {
        RenderPipeline.LoadContent();
    }

    internal static void Update()
    {
        _game.IsMouseVisible = !Global.GameWindowFocused;

        if(!_focusClicked)
        {
            bool disabled = Input.InputDisabled;
            Input.InputDisabled = false;
            _focusClicked = Input.GetAnyPressed(InputType.Mouse);
            Input.InputDisabled = disabled;
        }

        if(!_game.IsActive)
            _focusClicked = false;

        EcsManager.Update();

        RenderPipeline.Camera.Update();

        RenderPipeline.LoadTextures();
    }

    internal static void Draw()
    {
        EcsManager.GetDrawables(RenderPipeline.GraphicsDevice);

        RenderPipeline.Draw();
    }

    internal static void ProcessExited()
    {
        if(_exited)
            return;
        _exited = true;

        if(!_game.IsDisposed)
        {
            // the game is accessible
        }

        // assume game is disposed here!

        Console.WriteLine();

        EcsManager.Cleanup();

        // other on-exit code
    }
}
