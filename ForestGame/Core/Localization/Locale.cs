using System.Text.Json;
using System.Text.Json.Serialization;

using ForestGame.Core.Localization;

namespace ForestGame.Core;

public static class Locale
{
    private const string ErrorMessage = "Fatal error while loading localization data";

    private static readonly Dictionary<string, LanguageSettings> loadedLanguages = [];

    private static readonly Dictionary<string, string> cachedValues = [];

    private static bool _initializing = false;

    private static string _lang;

    private static LocalizationOptions _currentOptions = LocalizationOptions.Default;

    public static Dictionary<string, LanguageSettings> AdditionalLanguageData { get; } = [];

    public static string LocalizationDataPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, _currentOptions.RelativeDataPath);

    public static bool IsInitialized { get; private set; } = false;

    public static event Action LocalizationDataReloaded;

    public static string CurrentLanguage
    {
        get => _lang;
        set
        {
            value ??= "en-us";

            if (_lang == value) return;

            Reload();

            if (loadedLanguages.ContainsKey(value))
                _lang = value;
            else
                throw new InvalidOperationException("Unknown language identifier");
        }
    }

    public static JsonSerializerOptions SerializerOptions => new()
    {
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        ReadCommentHandling = JsonCommentHandling.Skip,
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        TypeInfoResolver = LanguageSettingsSourceGenContext.Default,
    };

    public static void SetDataPath(string path)
    {
        if(!Directory.Exists(path))
            Console.WriteLine("LocalizationManager warning: setting localization data path to a directory that does not exist yet!");

        _currentOptions.RelativeDataPath = Path.TrimEndingDirectorySeparator(path);
    }

    public static void Reload()
    {
        ReloadAsync().Wait();
    }

    public static async Task ReloadAsync()
    {
        if (_initializing) return;
        _initializing = true;

        IsInitialized = false;
        Console.WriteLine("Reloading localization data...");

        loadedLanguages.Clear();
        cachedValues.Clear();

        string dataPathValue = LocalizationDataPath;

        if(!Directory.Exists(dataPathValue))
        {
            throw new DirectoryNotFoundException($"{ErrorMessage}: path does not exist: {dataPathValue}");
        }

        IEnumerable<string> files = Directory.EnumerateFiles(
            dataPathValue, "*.json",
            _currentOptions.SearchRecursively
                ? SearchOption.AllDirectories
                : SearchOption.TopDirectoryOnly
        );

        foreach (var fullPath in files)
        {
            var text = await File.ReadAllTextAsync(fullPath);

            LanguageSettings? data = (LanguageSettings?)JsonSerializer.Deserialize(text, typeof(LanguageSettings), SerializerOptions);
            if(data is null)
                continue;

            data.Identifier = Path.GetFileNameWithoutExtension(fullPath);
            data.Name ??= data.Identifier;

            loadedLanguages.Add(data.Identifier, data);
        }

        foreach (var item in AdditionalLanguageData)
        {
            // merge matching items (AdditionalLanguageData items take priority)
            if (loadedLanguages.TryGetValue(item.Key, out LanguageSettings? settings) && item.Key == (item.Value.Identifier ?? item.Key))
            {
                if (item.Value.Fallback is not null) settings.Fallback = item.Value.Fallback;

                if (item.Value.Values is not null)
                {
                    foreach (var keyValue in item.Value.Values)
                    {
                        settings.Values[keyValue.Key] = keyValue.Value;
                    }
                }
            }
            else
                loadedLanguages[item.Key] = item.Value; // add unique items
        }

        _initializing = false;
        IsInitialized = true;

        Console.WriteLine("Localization data reloaded");

        if (loadedLanguages.Count > 0)
        {
            string str = "";
            foreach (string key in loadedLanguages.Keys)
            {
                str += $"\n  - {loadedLanguages[key].Name ?? key}";
                if (loadedLanguages[key].Name is not null) str += $" ({key})";
            }

            Console.WriteLine("Loaded languages:" + str);
        }

        LocalizationDataReloaded?.Invoke();
    }

    public static void Configure(LocalizationOptions? options)
    {
        _currentOptions = options ?? LocalizationOptions.Default;
    }

    public static bool TokenExists(string token) => Get(token) != token;

    public static string Get(string token)
    {
        if (!IsInitialized)
        {
            _ = ReloadAsync();
            return token;
        }

        if (cachedValues.TryGetValue(token, out string? value))
        {
            return value;
        }

        if (_lang is not null)
        {
            return TryGetValue(token, _lang);
        }

        return token;
    }

    public static bool TryGet(string token, out string value)
    {
        value = Get(token);
        return value != token;
    }

    private static string TryGetValue(string token, string langId)
    {
        List<string> failedLangs = [];

        if (!loadedLanguages.TryGetValue(langId, out LanguageSettings? languageSettings))
        {
            cachedValues.Add(token, token);
            return token;
        }

        if (languageSettings.Values is null || !languageSettings.Values.TryGetValue(token, out string? value))
        {
            if (languageSettings.Fallback is null || languageSettings.Fallback == langId || failedLangs.Contains(languageSettings.Fallback))
            {
                cachedValues.Add(token, token);
                return token;
            }

            failedLangs.Add(langId);
            return TryGetValue(token, languageSettings.Fallback);
        }

        cachedValues.Add(token, value);
        return value;
    }

    public static IEnumerable<LanguageSettings> LoadedLanguages => loadedLanguages.Values;
}
