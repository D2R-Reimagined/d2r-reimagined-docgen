using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Notification;
using Avalonia.Platform.Storage;
using ReimaginedDocgen.Generators;
using ReimaginedDocgen.Utilities;

namespace ReimaginedDocgen;

public partial class MainWindow : Window
{
    public static INotificationMessageManager ManagerInstance { get; } = new NotificationMessageManager();
    public static AppSettings Settings = new();

    public MainWindow()
    {
        InitializeComponent();
        _ = LoadSettingsAsync();
    }
    
    private async Task LoadSettingsAsync()
    {
        Settings = await SettingsManager.LoadAsync();
    
        if (!string.IsNullOrWhiteSpace(Settings.InputDirectory))
            DirectoryTextBox.Text = Settings.InputDirectory;

        if (!string.IsNullOrWhiteSpace(Settings.OutputDirectory))
            OutputDirectoryTextBox.Text = Settings.OutputDirectory;
        
        LanguageComboBox.SelectedItem = LanguageComboBox.Items
            .OfType<ComboBoxItem>()
            .FirstOrDefault(i => (string)i.Content == Settings.Language);
    }
    
    private async void OnBrowseClick(object? sender, RoutedEventArgs e)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Input Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            var path = folders[0].Path.LocalPath;
            DirectoryTextBox.Text = path;
            Settings.InputDirectory = path;
            await SettingsManager.SaveAsync(Settings);
        }
    }


    private async void OnBrowseOutputClick(object? sender, RoutedEventArgs e)
    {
        var folders = await this.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Output Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            var path = folders[0].Path.LocalPath;
            OutputDirectoryTextBox.Text = path;
            Settings.OutputDirectory = path;
            await SettingsManager.SaveAsync(Settings);
        }
    }

    private async void OnRunClick(object? sender, RoutedEventArgs e)
    {
        var inputDirectory = DirectoryTextBox.Text;
        var outputDirectory = OutputDirectoryTextBox.Text;

        if (string.IsNullOrWhiteSpace(inputDirectory) || string.IsNullOrWhiteSpace(outputDirectory))
        {
            Notifications.SendNotification("Please select both input and output directories.", "Warning");
            return;
        }

        try
        {
            await new CubeMainGenerator().GenerateCubeMainJson(inputDirectory, outputDirectory);
            Notifications.SendNotification("Cube Main Parsed", "Success");
        }
        catch (Exception ex)
        {
            Notifications.SendNotification($"Cube Main Error: {ex.Message}", "Error");
            return;
        }

        try
        {
            await new UniqueItemsGenerator().GenerateCubeMainJson(inputDirectory, outputDirectory);
            Notifications.SendNotification("Unique Items Parsed", "Success");
        }
        catch (Exception ex)
        {
            Notifications.SendNotification($"Unique Items Error: {ex.Message}", "Error");
            return;
        }

        try
        {
            await new RunewordGenerator().GenerateRunewordJson(inputDirectory, outputDirectory);
            Notifications.SendNotification("Runeword Parsed", "Success");
        }
        catch (Exception ex)
        {
            Notifications.SendNotification($"Runeword Error: {ex.Message}", "Error");
        }
    }
    
    private async void OnLanguageChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (LanguageComboBox.SelectedItem is ComboBoxItem item && item.Content is string langCode)
        {
            Settings.Language = langCode;
            await SettingsManager.SaveAsync(Settings);
        }
    }

}

