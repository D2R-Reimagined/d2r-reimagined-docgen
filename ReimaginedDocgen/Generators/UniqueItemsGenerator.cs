﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using D2RReimaginedTools.Helpers;
using D2RReimaginedTools.JsonFileParsers;
using D2RReimaginedTools.TextFileParsers;
using ReimaginedDocgen.Utilities;

namespace ReimaginedDocgen.Generators;

public class UniqueItemsGenerator
{
    public async Task GenerateCubeMainJson(string directory, string outputDirectory)
    {
        var path = Path.Combine(directory, "global", "excel");
        var uniqueItems = Path.Combine(path, "uniqueitems.txt");
        if (!File.Exists(uniqueItems))
        {
            Notifications.SendNotification($"Could not find {uniqueItems}", "Warning");
            return;
        }

        var propertiesPath = Path.Combine(path, "properties.txt");
        if (!File.Exists(propertiesPath))
        {
            Notifications.SendNotification($"Could not find {propertiesPath}", "Warning");
            return;
        }

        var modifiersPath = Path.Combine(directory, "local", "lng", "strings", "item-modifiers.json");
        if (!File.Exists(modifiersPath))
        {
            Notifications.SendNotification($"Could not find {modifiersPath}", "Warning");
            return;
        }

        Notifications.SendNotification("Parsing uniqueitems.txt...", "Info");
        var entries = await UniqueItemsParser.GetEntries(uniqueItems);
        Notifications.SendNotification("Parsing properties.txt...", "Info");
        var allProperties = await PropertiesParser.GetEntries(propertiesPath);
        Notifications.SendNotification("Loading item-modifiers.json...", "Info");
        var parser = new TranslationFileParser(modifiersPath);
        
        var outputData = new List<object>();
        int entryCount = 0;
        foreach (var entry in entries)
        {
            entryCount++;
            if (entryCount % 100 == 0)
                Notifications.SendNotification($"Processing unique item {entryCount}...", "Info");
            var item = new
            {
                Index = entry.Index,
                ItemName = entry.ItemName,
                Code = entry.Code,
                Properties = new List<object>(),
            };

            if (entry.Properties == null)
            {
                outputData.Add(item);
                continue;
            }

            foreach (var property in entry.Properties)
            {
                var foundProperty = allProperties.FirstOrDefault(p => p.Code == property.Property);
                var propertyName = property.Property;
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
        Notifications.SendNotification($"Wrote {outputData.Count} unique items to uniques.json", "Success");
    }
}

