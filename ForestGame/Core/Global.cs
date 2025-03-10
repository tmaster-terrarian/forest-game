using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

public static class Global
{
    public static bool GameWindowFocused => Internals._game.IsActive && Internals._focusClicked;
}
