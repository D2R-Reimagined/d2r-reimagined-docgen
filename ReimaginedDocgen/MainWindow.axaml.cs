using System;
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
    private AppSettings _settings = new();

    public MainWindow()
    {
        InitializeComponent();
        _ = LoadSettingsAsync();
    }
    
    private async Task LoadSettingsAsync()
    {
        _settings = await SettingsManager.LoadAsync();
    
        if (!string.IsNullOrWhiteSpace(_settings.InputDirectory))
            DirectoryTextBox.Text = _settings.InputDirectory;

        if (!string.IsNullOrWhiteSpace(_settings.OutputDirectory))
            OutputDirectoryTextBox.Text = _settings.OutputDirectory;
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
            _settings.InputDirectory = path;
            await SettingsManager.SaveAsync(_settings);
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
            _settings.OutputDirectory = path;
            await SettingsManager.SaveAsync(_settings);
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
        
        await new CubeMainGenerator().GenerateCubeMainJson(inputDirectory, outputDirectory);
        Notifications.SendNotification("Cube Main Parsed", "Success");
        
        await new UniqueItemsGenerator().GenerateCubeMainJson(inputDirectory, outputDirectory);
        Notifications.SendNotification("Unique Items Parsed", "Success");
    }
}