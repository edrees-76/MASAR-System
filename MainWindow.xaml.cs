using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using MASAR.ViewModels;
using MASAR.Helpers;

namespace MASAR;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = App.ServiceProvider.GetRequiredService<MainViewModel>();
        
        // تطبيق اتجاه الواجهة بناءً على اللغة المحفوظة عند التشغيل
        this.FlowDirection = SettingsHelper.Language == "ar" ? FlowDirection.RightToLeft : FlowDirection.LeftToRight;
    }

    protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
    {
        var viewModel = DataContext as MainViewModel;
        
        // إذا كان المستخدم مسجلاً دخوله، نقوم بتحويل "الإغلاق" إلى "تسجيل خروج"
        if (viewModel != null && viewModel.IsLoggedIn)
        {
            e.Cancel = true; // نمنع إغلاق النافذة فوراً
            viewModel.LogoutCommand.Execute(null); // نفتح رسالة التأكيد الموحدة
        }
        else
        {
            // إذا كان في واجهة الدخول، يتم إغلاق البرنامج بشكل طبيعي
            base.OnClosing(e);
        }
    }
}
