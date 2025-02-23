using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace ForestGame.Core;

public abstract class MappedInput
{
    public abstract bool IsDown { get; }

    public abstract bool Pressed { get; }

    public abstract bool Released { get; }

    public class Keyboard(Keys key) : MappedInput
    {
        public Keys Key => key;

        public override bool IsDown => Input.GetDown(key);

        public override bool Pressed => Input.GetPressed(key);

        public override bool Released => Input.GetReleased(key);

        public override string ToString()
        {
            return $"KB:{key}";
        }
    }

    public class GamePad(Buttons button, PlayerIndex playerIndex) : MappedInput
    {
        public Buttons Button => button;

        public override bool IsDown => Input.GetDown(button, playerIndex);

        public override bool Pressed => Input.GetPressed(button, playerIndex);

        public override bool Released => Input.GetReleased(button, playerIndex);

        public override string ToString()
        {
            return $"GP:{button}";
        }
    }

    public class Mouse(MouseButtons mouseButton) : MappedInput
    {
        public MouseButtons MouseButton => mouseButton;

        public override bool IsDown => Input.GetDown(mouseButton);

        public override bool Pressed => Input.GetPressed(mouseButton);

        public override bool Released => Input.GetReleased(mouseButton);

        public override string ToString()
        {
            return $"MB:{mouseButton}";
        }
    }
}
