using System.Text.Json.Serialization;

namespace ForestGame.Core.Serialization;

public static class StageSerializer
{
    public static Stage Load(string filePath)
    {
        string path = filePath.EndsWith(".json") ? filePath : filePath + ".json";
        
        

        return null!;
    }
}

[JsonSerializable(typeof(Stage))]
[JsonSerializable(typeof(Dictionary<string, string>))]
[JsonSerializable(typeof(List<int>))]
internal partial class StageSourceGenContext : JsonSerializerContext;
