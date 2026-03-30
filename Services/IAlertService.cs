using System;
using System.Collections.Generic;
using MASAR.Models;

namespace MASAR.Services;

public interface IAlertService
{
    List<AlertNotification> GenerateAlerts();
    List<AlertNotification> GetActiveAlerts();
    int GetUnreadCount();
    void MarkAsRead(Guid alertId);
    void DismissAlert(Guid alertId);
    void MarkAllAsRead();
}
