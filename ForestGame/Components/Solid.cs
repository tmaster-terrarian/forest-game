using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public record struct Solid
{
    public Vector3 Position { get; set; }
    public Collider Collider { get; set; }

    public void Update()
    {

    }
}
