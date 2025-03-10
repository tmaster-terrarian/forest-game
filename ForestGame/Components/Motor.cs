using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public record struct Motor()
{
    public float MaxSpeed { get; set; }
    public Vector3 MovementDirection { get; set; }
    public float Yaw { get; set; }
    public float Pitch { get; set; }
}
