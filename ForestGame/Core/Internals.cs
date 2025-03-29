using Arch.Core;
using ForestGame.Core.Graphics;
using ForestGame.Core.UI;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core;

public static class Internals
{
    internal static Game1 _game;
    internal static bool _focusClicked = false;
    internal static bool _focusClickedChanged = false;

    private static bool _exited = false;

    internal static void Initialize()
    {
        ContentLoader.Initialize(_game.Content, _game.GraphicsDevice);

        RenderPipeline.Initialize(_game.Window, _game.GraphicsDevice);

        ImGuiManager.Initialize(_game);

        Locale.Configure(new() {
            RelativeDataPath = "data/lang",
            SearchRecursively = false,
        });
        Locale.CurrentLanguage = "en-us";

        EcsManager.Start();

        Registry.Initialize();
    }

    internal static void LoadContent()
    {
        RenderPipeline.LoadContent();
    }

    internal static void Update()
    {
        _focusClickedChanged = false;
        Global.EditorOpened = false;
        Global.EditorClosed = false;

        if(!_focusClicked)
        {
            bool disabled = Input.InputDisabled;
            Input.InputDisabled = false;
            bool value = Input.GetAnyPressed(InputType.Mouse) || Global.Editor;
            _focusClickedChanged = _focusClicked != value;
            _focusClicked = value;
            Input.InputDisabled = disabled;
        }

        if(!_game.IsActive || (Input.GetPressed(Keys.Escape) && !Global.Editor))
        {
            if(_focusClickedChanged)
                _focusClickedChanged = false;
            _focusClicked = false;
        }

        if(Global.Editor)
        {
            Global.LockMouse = Input.GetDown(MouseButtons.MiddleButton);
        }

        if(Input.GetPressed(Keys.F3))
        {
            Global.Editor = !Global.Editor;
            Global.LockMouse = !Global.Editor;

            if(Global.Editor)
                Global.EditorOpened = true;
            else
                Global.EditorClosed = true;
        }

        _game.IsMouseVisible = (!Global.GameWindowFocused ^ !Global.LockMouse) || (Global.Editor && !Global.LockMouse);

        EcsManager.Update();

        RenderPipeline.Camera.Update();

        RenderPipeline.LoadTextures();
    }

    internal static void Draw()
    {
        EcsManager.GetDrawables(RenderPipeline.GraphicsDevice);

        RenderPipeline.Draw();
    }

    internal static void PostDraw()
    {
        ImGuiManager.Draw();
    }

    internal static void Cleanup()
    {
        StageManager.Cleanup();

        ModelAspect.Cleanup();

        foreach(var aspect in Registry<Aspect>.Registered)
        {
            aspect.Clear();
        }

        GC.Collect();

        EcsManager.Restart();
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
