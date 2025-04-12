using System;
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

    public MainWindow()
    {
        InitializeComponent();
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
            DirectoryTextBox.Text = folders[0].Path.LocalPath;
        }
    }


    private async void OnBrowseOutputClick(object? sender, RoutedEventArgs e)
    {
        var folders = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Select Output Folder",
            AllowMultiple = false
        });

        if (folders.Count > 0)
        {
            OutputDirectoryTextBox.Text = folders[0].Path.LocalPath;
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