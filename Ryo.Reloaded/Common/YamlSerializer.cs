using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;

namespace Ryo.Reloaded.Common;

public static class YamlSerializer
{
    private static readonly IDeserializer deserializer = new DeserializerBuilder()
        .WithNamingConvention(UnderscoredNamingConvention.Instance)
        .Build();

    public static T DeserializeFile<T>(string filePath)
        => deserializer.Deserialize<T>(File.ReadAllText(filePath));
}
