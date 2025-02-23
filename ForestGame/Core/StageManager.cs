namespace ForestGame.Core;

public static class StageManager
{
    private static readonly List<Stage> _activeStages = [];

    public static void Initialize()
    {
        var stage = ContentLoader.Load<Stage>("stages/test");
    }
}
