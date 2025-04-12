using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using D2RReimaginedTools.FileParsers;
using ReimaginedDocgen.Utilities;

namespace ReimaginedDocgen.Generators;

public class UniqueItemsGenerator
{
    public async Task GenerateCubeMainJson(string directory, string outputDirectory)
    {
        var path = Path.Combine(directory, "global", "excel", "uniqueitems.txt");
        if (!File.Exists(path))
        {
            Notifications.SendNotification($"Could not find {path}", "Warning");
            return;
        }

        var cubeEntries = await UniqueItemsParser.GetEntries(path);
        
        var outputData = cubeEntries.Select(entry =>
        {
            var item = new
            {
                Index = entry.Index,
                ItemName = entry.ItemName,
                Code = entry.Code,
                Properties = new List<object>(),
            };
            
            if (entry.Properties == null) return item;
            
            foreach (var property in entry.Properties)
            {
                var propertyName = property.Property;
                if (string.IsNullOrWhiteSpace(propertyName))
                {
                    continue;
                }
                
                var propertyMin = property.Min;
                var propertyMax = property.Max;
                item.Properties.Add(new
                {
                    PropertyName = propertyName,
                    PropertyMin = propertyMin,
                    PropertyMax = propertyMax
                });
            }

            return item;
        }).ToList();

        var outputPath = Path.Combine(outputDirectory, "uniques.json");
        var json = JsonSerializer.Serialize(outputData, SerializerOptions.Indented);

        await File.WriteAllTextAsync(outputPath, json);
    }
}