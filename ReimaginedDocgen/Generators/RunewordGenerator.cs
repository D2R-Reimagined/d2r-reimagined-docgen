using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using D2RReimaginedTools.Helpers;
using D2RReimaginedTools.JsonFileParsers;
using D2RReimaginedTools.TextFileParsers;
using ReimaginedDocgen.Utilities;

namespace ReimaginedDocgen.Generators;

public class RunewordGenerator
{
    public async Task GenerateRunewordJson(string directory, string outputDirectory)
    {
        var path = Path.Combine(directory, "global", "excel");
        var uniqueItems = Path.Combine(path, "runes.txt");
        if (!File.Exists(uniqueItems))
        {
            Notifications.SendNotification($"Could not find {uniqueItems}", "Warning");
            return;
        }
        
        var entries = await RunesParser.GetEntries(uniqueItems);
        
        var allProperties = await PropertiesParser.GetEntries(Path.Combine(path, "properties.txt"));
        var parser = new TranslationFileParser(Path.Combine(directory, "local", "lng", "strings", "item-runes.json"));
        
        var outputData = new List<object>();
        foreach (var entry in entries)
        {
            var item = new
            {
                Runes = new List<object>(),
                Types = new List<object>(),
                Name = entry.Name,
                Enabled = false,
                Properties = new List<object>(),
            };
            
            foreach (var property in entry.Mods)
            {
                var foundProperty = allProperties.FirstOrDefault(p => p.Code == property.Code);
                var propertyName = property.Code;
                if (foundProperty != null)
                {
                    var translation = await PropertiesHelper.GetPropertyTranslationKeyAsync(path, foundProperty);
                    if (!translation.Contains("NOT FOUND"))
                    {
                        var translationResult = await parser.GetTranslationByKeyAsync(translation);
                        propertyName = translationResult.EnUS;
                    }
                }

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
            
            outputData.Add(item);
        }

        var outputPath = Path.Combine(outputDirectory, "uniques.json");
        var json = JsonSerializer.Serialize(outputData, SerializerOptions.Indented);

        await File.WriteAllTextAsync(outputPath, json);
    }
}