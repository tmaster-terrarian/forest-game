using Microsoft.Xna.Framework.Graphics;

namespace ForestGame.Core;

/// <summary>
/// The S in ECS.
/// </summary>
public interface ISystem
{
    public void Update();

    /// <summary>
    /// Also call Update when the Editor is enabled.
    /// </summary>
    public interface EditorUpdate : ISystem;

    /// <summary>
    /// Submit draw calls to Aspects before the Draw phase.
    /// </summary>
    public interface Drawable : ISystem
    {
        public void GetDrawables(GraphicsDevice graphicsDevice);
    }
}
