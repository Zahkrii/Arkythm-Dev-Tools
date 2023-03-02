using ChartConvertor.Models;
using System.Collections.ObjectModel;
using ChartConvertor.ViewModels;

using Microsoft.UI.Xaml.Controls;
using System.Diagnostics;
using Microsoft.UI.Xaml;

namespace ChartConvertor.Views;

public sealed partial class MainPage : Page
{
    public MainViewModel ViewModel
    {
        get;
    }

    public MainPage()
    {
        ViewModel = App.GetService<MainViewModel>();
        InitializeComponent();
    }

    private void Button_GotFocus(object sender, RoutedEventArgs e)
    {
        var item = (sender as FrameworkElement).Tag as ObservableChartListItem;
        if (MainList.SelectedItem != item)
            MainList.SelectedItem = item;
    }
}