using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChartConvertor.ViewModels;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;

namespace ChartConvertor.Models;

public class ObservableChartListItem : ObservableObject
{
    private string _fileName;
    private short _id;
    private string _name;
    private short _difficulty;
    private short _level;
    private string _path;

    private MainViewModel _mainViewModel;

    public ObservableChartListItem(MainViewModel mainViewModel)
    {
        _mainViewModel = mainViewModel;
        RemoveFileCommand = new RelayCommand<object>(RemoveFile);
    }

    public ObservableChartListItem(MainViewModel mainViewModel, string fileName, short id, string name, short difficulty, short level, string path)
    {
        _mainViewModel = mainViewModel;
        FileName = fileName;
        ID = id;
        Name = name;
        Difficulty = difficulty;
        Level = level;
        Path = path;
        RemoveFileCommand = new RelayCommand<object>(RemoveFile);
    }

    public string FileName
    {
        get => _fileName;
        set => SetProperty(ref _fileName, value);
    }

    public short ID
    {
        get => _id;
        set => SetProperty(ref _id, value);
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public short Difficulty
    {
        get => _difficulty;
        set => SetProperty(ref _difficulty, value);
    }

    public short Level
    {
        get => _level;
        set => SetProperty(ref _level, value);
    }

    public string Path
    {
        get => _path;
        set => SetProperty(ref _path, value);
    }

    public RelayCommand<object> RemoveFileCommand
    {
        get;
    }

    private void RemoveFile(object obj)
    {
        _mainViewModel.ChartList.Remove(obj as ObservableChartListItem);
        if (_mainViewModel.ChartList.Count < 1)
        {
            _mainViewModel.ButtonEnabled = false;
        }
    }
}