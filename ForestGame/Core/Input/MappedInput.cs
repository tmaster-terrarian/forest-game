using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core.Input;

public abstract class MappedInput
{
    public abstract bool IsDown { get; }

    public abstract bool Pressed { get; }

    public abstract bool Released { get; }

    public class Keyboard(Keys key) : MappedInput
    {
        public Keys Key => key;

        public override bool IsDown => InputManager.GetDown(key);

        public override bool Pressed => InputManager.GetPressed(key);

        public override bool Released => InputManager.GetReleased(key);

        public override string ToString()
        {
            return $"KB:{key}";
        }
    }

    public class GamePad(Buttons button, PlayerIndex playerIndex) : MappedInput
    {
        public Buttons Button => button;

        public override bool IsDown => InputManager.GetDown(button, playerIndex);

        public override bool Pressed => InputManager.GetPressed(button, playerIndex);

        public override bool Released => InputManager.GetReleased(button, playerIndex);

        public override string ToString()
        {
            return $"GP:{button}";
        }
    }

    public class Mouse(MouseButtons mouseButton) : MappedInput
    {
        public MouseButtons MouseButton => mouseButton;

        public override bool IsDown => InputManager.GetDown(mouseButton);

        public override bool Pressed => InputManager.GetPressed(mouseButton);

        public override bool Released => InputManager.GetReleased(mouseButton);

        public override string ToString()
        {
            return $"MB:{mouseButton}";
        }
    }
}
