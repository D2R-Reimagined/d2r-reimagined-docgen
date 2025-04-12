using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia.Notification;
using D2RReimaginedTools.TextFileParsers;
using ReimaginedDocgen.Utilities;

namespace ReimaginedDocgen.Generators;

public class CubeMainGenerator
{
    public async Task GenerateCubeMainJson(string directory, string outputDirectory)
    {
        var path = Path.Combine(directory, "global", "excel", "cubemain.txt");
        if (!File.Exists(path))
        {
            Notifications.SendNotification($"Could not find {path}", "Warning");
            return;
        }

        var cubeEntries = await CubeMainParser.GetEntries(path);
        
        var outputData = cubeEntries.Select(entry => new
        {
            Description = entry.Description,
            Output = "", // placeholder
            Input = "",  // placeholder
            CubeRecipeDescription = "" // placeholder
        }).ToList();

        var outputPath = Path.Combine(outputDirectory, "cube-recipes.json");
        var json = JsonSerializer.Serialize(outputData, SerializerOptions.Indented);

        await File.WriteAllTextAsync(outputPath, json);
    }
}