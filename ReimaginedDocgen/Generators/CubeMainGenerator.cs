using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using D2RReimaginedTools.TextFileParsers;

namespace ReimaginedDocgen.Generators;

public class CubeMainGenerator
{
    public async Task GenerateCubeMainJson(string directory, string outputDirectory)
    {
        var cubeTxtPath = Path.Combine(directory, "global", "excel", "cubemain.txt");
        if (!File.Exists(cubeTxtPath))
        {
            return;
        }

        var cubeEntries = await CubeMainParser.GetEntries(cubeTxtPath);
        
        var outputData = cubeEntries.Select(entry => new
        {
            Description = entry.Description,
            Item = (string?)null,
            Output = "", // placeholder
            Input = "",  // placeholder
            CubeRecipeDescription = "" // placeholder
        }).ToList();

        var outputPath = Path.Combine(outputDirectory, "cube-recipes.json");
        var json = JsonSerializer.Serialize(outputData, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        await File.WriteAllTextAsync(outputPath, json);
    }
}