namespace ForestGame.Components;

public record struct PrototypeIdentity(string Id)
{
    public static implicit operator PrototypeIdentity(string id)
        => new(id);
}
