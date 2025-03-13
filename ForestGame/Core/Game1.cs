using Arch.Core.Extensions;
using ForestGame.Components;
using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core;

internal class Game1 : Game
{
    internal readonly GraphicsDeviceManager _graphics;
    private bool _disposed;

    public bool IsDisposed => _disposed;

    public Game1()
    {
        Internals._game = this;

        _graphics = new GraphicsDeviceManager(this);

        Content.RootDirectory = "shaders";

        IsMouseVisible = false;
        IsFixedTimeStep = false;

        Disposed += (object? sender, EventArgs args) => _disposed = true;
    }

    protected override void Initialize()
    {
        Internals.Initialize();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        Internals.LoadContent();
        base.LoadContent();
    }

    protected override void BeginRun()
    {
        StageManager.Load(Registries.Stages.Test);
        base.BeginRun();
    }

    protected override void Update(GameTime gameTime)
    {
        Time.Update(gameTime);

        Input.InputDisabled = !Global.GameWindowFocused;
        Input.RefreshMouseState();
        Input.RefreshKeyboardState();
        Input.RefreshGamePadState();
        Input.UpdateTypingInput(gameTime);

        if (Input.GetPressed(Buttons.Back) || Input.GetPressed(Keys.Escape))
            Exit();

        Internals.Update();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        Internals.Draw();
        base.Draw(gameTime);
    }

    protected override void EndRun()
    {
        Internals.ProcessExited();
    }

    protected override void OnExiting(object sender, ExitingEventArgs args)
    {
        base.OnExiting(sender, args);
        if(!args.Cancel)
            Internals.ProcessExited();
    }
}
