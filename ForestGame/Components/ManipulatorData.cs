using Arch.Core;

namespace ForestGame.Components;

public record struct ManipulatorData(EntityReference TargetEntity)
{
    public ManipulatorData() : this(EntityReference.Null) { }
}
