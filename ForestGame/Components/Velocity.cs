using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public record struct Velocity(Vector3 Delta) : IRequiresTransform;
