using ForestGame.Core;
using Microsoft.Xna.Framework;

namespace ForestGame.Components;

public record struct CameraTarget(Transform Anchor, CameraAnchorFlags Flags);

[Flags]
public enum CameraAnchorFlags
{
    None = 0,
    AllowLookHorizontal = 1,
    AllowLookVertical = 2,
}
