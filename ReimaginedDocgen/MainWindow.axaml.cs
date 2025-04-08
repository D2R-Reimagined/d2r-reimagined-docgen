using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Notification;
using Avalonia.Platform.Storage;
using ReimaginedDocgen.Generators;

namespace ReimaginedDocgen;

public partial class MainWindow : Window
{
    public static INotificationMessageManager ManagerInstance { get; } = new NotificationMessageManager();

    public MainWindow()
    {
        InitializeComponent();
    }

    private void SendNotification(string message, string badgeType = "Info")
    {
        ManagerInstance
            .CreateMessage()
            .Accent("#1751C3")
            .Animates(true)
            .Background("#333")
            .HasBadge(badgeType)
            .HasMessage(message)
            .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
            .Queue();
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
            SendNotification("Please select both input and output directories.", "Warning");
            return;
        }

        var generator = new CubeMainGenerator();
        await generator.GenerateCubeMainJson(inputDirectory, outputDirectory);
        SendNotification("Cube Main Parsed", "Success");
    }
}