using System.Windows.Controls;

namespace MASAR.Views;

public partial class SourcesView : UserControl
{
    public SourcesView()
    {
        InitializeComponent();
    }

    private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
    {
        e.Row.Header = (e.Row.GetIndex() + 1).ToString();
    }
}
