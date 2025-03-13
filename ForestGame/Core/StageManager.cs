namespace ForestGame.Core;

public static class StageManager
{
    public static Stage? Active { get; private set; }

    public static void Load(string id)
    {
        if(Active is not null)
        {
            Internals.Cleanup();
        }

        if(id is null)
            return;

        Active = Registry<Stage>.Get(id);
        Stage.DoStart(Active);
    }

    internal static void Cleanup()
    {
        if(Active is null)
            return;

        Stage.DoEnd(Active);
        Active = null;
    }
}
