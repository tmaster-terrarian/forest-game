namespace ForestGame.Core.Localization;

public struct LocalizationOptions()
{
    private static readonly LocalizationOptions _default = new();

    public static LocalizationOptions Default => _default;

    public bool SearchRecursively { get; set; } = false;

    public string RelativeDataPath { get; set; } = "Content/localizations";
}
