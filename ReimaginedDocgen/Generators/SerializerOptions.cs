using System.Text.Json;

namespace ReimaginedDocgen.Generators;

public static class SerializerOptions
{
    public static JsonSerializerOptions Indented = new JsonSerializerOptions
    {
        WriteIndented = true
    };
}