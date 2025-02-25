using Microsoft.Xna.Framework;

namespace ForestGame.Core.Components;

public record struct Velocity(Vector3 Vector) : IRequiresTransform;
