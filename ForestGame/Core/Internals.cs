using ForestGame.Core.Graphics;
using Microsoft.Xna.Framework;

namespace ForestGame.Core;

public static class Internals
{
    internal static void Initialize(Game game)
    {
        ContentLoader.Initialize(game.Content, game.GraphicsDevice);

        RenderPipeline.Initialize(game.Window, game.GraphicsDevice);

        Locale.Configure(new() {
            RelativeDataPath = "data/lang",
            SearchRecursively = false,
        });
        Locale.CurrentLanguage = "en-us";

        Registry.Initialize();

        EcsManager.Start();

        // StageManager.Initialize();
    }

    internal static void LoadContent(Game game)
    {
        RenderPipeline.LoadContent();
    }

    internal static void Update()
    {
        EcsManager.Update();

        RenderPipeline.Camera.Update();

        RenderPipeline.LoadTextures();
    }

    internal static void Draw()
    {
        EcsManager.GetDrawables(RenderPipeline.GraphicsDevice);

        RenderPipeline.Draw();
    }
}
