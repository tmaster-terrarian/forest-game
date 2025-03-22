using ForestGame.Core.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

public static class Global
{
    public static bool GameWindowFocused => Internals._game.IsActive && Internals._focusClicked;

    public static bool LockMouse { get; internal set; } = true;

    public static bool PlayerCanMove { get; internal set; } = true;
}
