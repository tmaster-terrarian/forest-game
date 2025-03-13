namespace ForestGame.Core;

public abstract class Stage
{
    internal static void DoStart(Stage stage)
    {
        stage.Start();
    }
    internal static void DoEnd(Stage stage)
    {
        stage.End();
    }

    /// <summary>
    /// override to initialize a stage
    /// </summary>
    protected virtual void Start() { }

    /// <summary>
    /// override to cleanup resources when the stage ends
    /// </summary>
    protected virtual void End() { }
}
