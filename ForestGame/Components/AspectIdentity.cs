namespace ForestGame.Components;

public record struct AspectIdentity(string Id)
{
    public static implicit operator AspectIdentity(string id)
        => new(id);
}
