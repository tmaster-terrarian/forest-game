using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public struct Pather
{
    public Pather()
    {
        TargetPosition = Vector3.Zero;
        LastPathTime = 0;
        PathUpdateInterval = 10;
        PathUpdateIntervalRandomRange = 5f;
        ForcePathUpdateOnDestinationReached = false;
    }
    public Vector3 TargetPosition { get; set; }
    public float LastPathTime { get; set; }
    public float PathUpdateInterval { get; set; }
    public float PathUpdateIntervalRandomRange { get; set; }
    public bool ForcePathUpdateOnDestinationReached { get; set; }
    public bool IsPathing { get; set; }
}
