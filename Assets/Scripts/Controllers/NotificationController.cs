using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Notifications.Android;
using Zenject;
using TheSTAR.Data;
using TheSTAR.Utility;

public class NotificationController
{
    [Inject] private readonly DataController _data;

    private AndroidNotificationChannel defaultChannel;

    private const string DefaultChannelID = "default_channel";

    private readonly Dictionary<NotificationType, NotificationData> notificationDatas = new ()
    {
        // информацию по нотификациям добавлять сюда
        { NotificationType.DailyBonus, new NotificationData("Daily Bonus Notification", "Description") }
    };

    public void Init()
    {
        defaultChannel = new AndroidNotificationChannel()
        {
            Id = DefaultChannelID,
            Name = "Default Channel",
            Description = "For Generic notifications",
            Importance = Importance.Default
        };

        AndroidNotificationCenter.RegisterNotificationChannel(defaultChannel);
    }

    public void RegisterNotification(NotificationType notificationType, DateTime time)
    {
        if (!_data.gameData.settingsData.isNotificationsOn) return;

        var notificationData = notificationDatas[notificationType];
        var notification = new AndroidNotification()
        {
            Title = notificationData.Title,
            Text = notificationData.Text,
            SmallIcon = notificationData.SmallIcon,
            LargeIcon = notificationData.LargeIcon,
            FireTime = time
        };

        CancelNotification(notificationType);
        var id = AndroidNotificationCenter.SendNotification(notification, DefaultChannelID);
        _data.gameData.notificationData.RegisterNotification(notificationType, id);

        Debug.Log($"Registered notification: {notificationType}, {time}");
    }

    #region Cancel

    public void CancelNotification(NotificationType notificationType)
    {
        var id = _data.gameData.notificationData.GetRegistredNotificationID(notificationType);
        if (id == -1) return;

        AndroidNotificationCenter.CancelNotification(id);
        _data.gameData.notificationData.ClearNotification(notificationType);
    }

    public void CancelAllNotifications()
    {
        var notificationTypes = EnumUtility.GetValues<NotificationType>();
        foreach (var n in notificationTypes) CancelNotification(n);
    }

    #endregion

    private struct NotificationData
    {
        public string Title;
        public string Text;
        public string SmallIcon;
        public string LargeIcon;

        public NotificationData(string title, string text)
        {
            Title = title;
            Text = text;
            SmallIcon = "default";
            LargeIcon = "default";
        }

        public NotificationData(string title, string text, string smallIcon, string largeIcon)
        {
            Title = title;
            Text = text;
            SmallIcon = smallIcon;
            LargeIcon = largeIcon;
        }
    }
}

public enum NotificationType
{
    DailyBonus
    // здесь добавить необходимые типы нотификаций
}