using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using MASAR.Helpers;
using MASAR.Services;
using MASAR.Interfaces;
using MASAR.Models;
using System;
using System.Windows;

namespace MASAR.ViewModels;

public partial class MainViewModel : ObservableObject
{
    private readonly IUserService _userService;
    private readonly IAlertService _alertService;

    [ObservableProperty] private ObservableObject? _currentView;
    [ObservableProperty] private string _currentViewName = "Dashboard";
    [ObservableProperty] private bool _isLoggedIn;
    [ObservableProperty] private string _currentUserName = string.Empty;
    [ObservableProperty] private string _currentUserRole = string.Empty;
    [ObservableProperty] private bool _isDarkMode;
    [ObservableProperty] private bool _isSidebarCollapsed;
    
    // ─── التنبيهات ───
    [ObservableProperty] private System.Collections.ObjectModel.ObservableCollection<MASAR.Models.AlertNotification> _notifications = new();
    [ObservableProperty] private int _unreadNotificationsCount;

    public MainViewModel(IUserService userService, IAlertService alertService)
    {
        _userService = userService;
        _alertService = alertService;
        IsDarkMode = SettingsHelper.IsDarkMode;
        ShowLogin();
    }

    public void ShowLogin()
    {
        IsLoggedIn = false;
        var loginVm = App.ServiceProvider.GetService(typeof(LoginViewModel)) as LoginViewModel;
        if (loginVm != null)
        {
            loginVm.LoginSuccess += OnLoginSuccess;
            CurrentView = loginVm;
        }
    }

    private void OnLoginSuccess()
    {
        IsLoggedIn = true;
        CurrentUserName = _userService.CurrentUser?.FullName ?? "";
        CurrentUserRole = _userService.CurrentUser?.Role?.RoleName ?? "";
        RefreshNotifications();
        RefreshSidebarPermissions();
        NavigateTo("Dashboard");
    }

    // ─── صلاحيات القائمة الجانبية ───
    [ObservableProperty] private bool _canSeeRadioisotopes = true;
    [ObservableProperty] private bool _canSeeSources = true;
    [ObservableProperty] private bool _canSeeLocations = true;
    [ObservableProperty] private bool _canSeeBorrowing = true;
    [ObservableProperty] private bool _canSeeReports = true;
    [ObservableProperty] private bool _canSeeUsers = true;
    [ObservableProperty] private bool _canSeeSettings = true;
    [ObservableProperty] private bool _canSeeCalculator = true;
    [ObservableProperty] private bool _canSeeHelp = true;

    private void RefreshSidebarPermissions()
    {
        var user = _userService.CurrentUser;
        if (user == null) return;
        CanSeeRadioisotopes = user.HasSectionPermission("Radioisotopes");
        CanSeeSources = user.HasSectionPermission("Sources");
        CanSeeLocations = user.HasSectionPermission("Locations");
        CanSeeBorrowing = user.HasSectionPermission("Borrowing");
        CanSeeReports = user.HasSectionPermission("Reports");
        CanSeeUsers = user.HasSectionPermission("Users");
        CanSeeSettings = user.HasSectionPermission("Settings");
        CanSeeCalculator = user.HasSectionPermission("ActivityCalculator");
    }

    [RelayCommand]
    public void RefreshNotifications()
    {
        if (!IsLoggedIn) return;
        var alerts = _alertService.GenerateAlerts();
        Notifications = new System.Collections.ObjectModel.ObservableCollection<MASAR.Models.AlertNotification>(alerts);
        UnreadNotificationsCount = _alertService.GetUnreadCount();
    }

    [RelayCommand]
    public void MarkNotificationAsRead(MASAR.Models.AlertNotification notification)
    {
        if (notification == null) return;
        _alertService.MarkAsRead(notification.Id);
        RefreshNotifications();
    }

    [RelayCommand]
    public void MarkAllNotificationsAsRead()
    {
        _alertService.MarkAllAsRead();
        RefreshNotifications();
    }

    [RelayCommand]
    public void NavigateTo(string viewName)
    {
        // التحقق من حالة التحرير في المنظور الحالي
        if (CurrentView is IEditableViewModel editable && editable.IsEditing)
        {
            DialogHelper.ShowWarning(TranslationHelper.GetString("MsgErrSavePending"), TranslationHelper.GetString("TitlePendingChanges"));
            return;
        }

        CurrentViewName = viewName;
        CurrentView = viewName switch
        {
            "Dashboard" => App.ServiceProvider.GetService(typeof(DashboardViewModel)) as ObservableObject,
            "Radioisotopes" => App.ServiceProvider.GetService(typeof(RadioisotopesViewModel)) as ObservableObject,
            "Sources" => App.ServiceProvider.GetService(typeof(SourcesViewModel)) as ObservableObject,
            "Locations" => App.ServiceProvider.GetService(typeof(LocationsViewModel)) as ObservableObject,
            "Borrowing" => App.ServiceProvider.GetService(typeof(BorrowViewModel)) as ObservableObject,
            "Reports" => App.ServiceProvider.GetService(typeof(ReportsViewModel)) as ObservableObject,
            "Users" => App.ServiceProvider.GetService(typeof(UsersViewModel)) as ObservableObject,
            "Settings" => App.ServiceProvider.GetService(typeof(SettingsViewModel)) as ObservableObject,
            "ActivityCalculator" => App.ServiceProvider.GetService(typeof(ActivityCalculatorViewModel)) as ObservableObject,
            "Help" => App.ServiceProvider.GetService(typeof(HelpViewModel)) as ObservableObject,
            "AboutSystem" => App.ServiceProvider.GetService(typeof(AboutSystemViewModel)) as ObservableObject,
            _ => CurrentView
        };
    }

    [RelayCommand]
    private void ToggleSidebar()
    {
        IsSidebarCollapsed = !IsSidebarCollapsed;
    }

    [RelayCommand]
    private void ToggleTheme()
    {
        IsDarkMode = !IsDarkMode;
        SettingsHelper.IsDarkMode = IsDarkMode;
        App.ApplyTheme(IsDarkMode);
    }

    [RelayCommand]
    private void Logout()
    {
        if (DialogHelper.ShowConfirmation(TranslationHelper.GetString("MsgConfirmLogout"), TranslationHelper.GetString("TitleLogout")))
        {
            _userService.Logout();
            CurrentUserName = string.Empty;
            CurrentUserRole = string.Empty;
            ShowLogin();
        }
    }
}
