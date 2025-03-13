using ForestGame.Core;
using ForestGame.Stages;

namespace ForestGame.Registries;

public static class Stages
{
    public const string Test = "test";

    public static void Initialize()
    {
        Registry.Register<Stage>(Test, new TestStage());
    }
}
