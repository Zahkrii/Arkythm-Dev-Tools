using ChartConvertor.ViewModels;

using Microsoft.UI.Xaml.Controls;

namespace ChartConvertor.Views;

public sealed partial class SaveFileViewPage : Page
{
    public SaveFileViewViewModel ViewModel
    {
        get;
    }

    public SaveFileViewPage()
    {
        ViewModel = App.GetService<SaveFileViewViewModel>();
        InitializeComponent();
    }
}
