using System;
using Avalonia.Notification;

namespace ReimaginedDocgen.Utilities;

public static class Notifications
{
    public static void SendNotification(string message, string badgeType = "Info")
    {
        MainWindow.ManagerInstance
            .CreateMessage()
            .Accent("#1751C3")
            .Animates(true)
            .Background("#333")
            .HasBadge(badgeType)
            .HasMessage(message)
            .Dismiss().WithDelay(TimeSpan.FromSeconds(5))
            .Queue();
    }
}