using Arch.Core;
using Arch.Core.Extensions;
using ForestGame.Components;

namespace ForestGame.Core;

public class Prototype
{
    public IList<object> Components { get; } = [];

    public virtual EntityReference Construct()
    {
        var entity = EcsManager.world.Create();
        foreach(var obj in Components)
            entity.Add(obj);
        entity.AddOrGet<PrototypeIdentity>().Id = Registry<Prototype>.GetKey(this)
            ?? throw new Exception("tried to construct entity from an unregistered prototype");
        return entity.Reference();
    }
}
