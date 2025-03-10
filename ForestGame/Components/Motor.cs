using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public record struct Motor()
{
    public float MaxSpeed { get; set; }
    public Vector3 MovementDirection { get; set; }
    public float Acceleration { get; set; }
    public float Deceleration { get; set; }
}
