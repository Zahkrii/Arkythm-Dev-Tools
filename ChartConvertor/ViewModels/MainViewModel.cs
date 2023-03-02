using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using Windows.Storage;
using Windows.Storage.Pickers;
using ChartConvertor.Helpers;
using ChartConvertor.Core.Services;
using ChartConvertor.Models;
using ChartConvertor.Core.Contracts.Services;
using ChartConvertor.Contracts.Services;
using ChartConvertor.Services;
using ProgressStage = ChartConvertor.Models.ProgressStageOptions.ProgressStage;
using System.Diagnostics;
using System.Data;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Collections;
using System.ComponentModel;
using Microsoft.VisualBasic;
using ChartConvertor.Core.Helpers;
using Newtonsoft.Json;

namespace ChartConvertor.ViewModels;

public class MainViewModel : ObservableRecipient
{
    private readonly IFileService _fileService;
    private readonly IProgressService _progressService;

    public MainViewModel()
    {
        OpenFileCommand = new AsyncRelayCommand(OpenFile);
        OpenFolderCommand = new AsyncRelayCommand(OpenFolder);
        StartConvertCommand = new RelayCommand(StartConvert);
        _fileService = new FileService();
        _progressService = new ProgressService();
        ChartList = new ObservableCollection<ObservableChartListItem>();
        ClearFileCommand = new RelayCommand<object>(ClearFile);
    }

    //列表
    private ObservableCollection<ObservableChartListItem> chartList;

    public ObservableCollection<ObservableChartListItem> ChartList
    {
        get => chartList;
        set => SetProperty(ref chartList, value);
    }

    private ObservableChartListItem selectedItem;

    public ObservableChartListItem SelectedItem
    {
        get => selectedItem;
        set => SetProperty(ref selectedItem, value);
    }

    //输出文件夹路径
    private string? outputFolderPath = null;

    public string? OutputFolderPath
    {
        get => outputFolderPath;
        set => SetProperty(ref outputFolderPath, value);
    }

    //进度条值
    private double progressValue = 0;

    public double ProgressValue
    {
        get => progressValue;
        set => SetProperty(ref progressValue, value);
    }

    //进度提示文本
    private string? progressText = null;

    public string? ProgressText
    {
        get => progressText;
        set => SetProperty(ref progressText, value);
    }

    //总进度条值
    private double progressValueTotal = 0;

    public double ProgressValueTotal
    {
        get => progressValueTotal;
        set => SetProperty(ref progressValueTotal, value);
    }

    //总进度提示文本
    private string? progressTextTotal = null;

    public string? ProgressTextTotal
    {
        get => progressTextTotal;
        set => SetProperty(ref progressTextTotal, value);
    }

    //进度指示圆环
    private Visibility progressRingVisibility = Visibility.Collapsed;

    public Visibility ProgressRingVisibility
    {
        get => progressRingVisibility;
        set => SetProperty(ref progressRingVisibility, value);
    }

    //转换按钮是否启用
    private bool buttonEnabled = true;

    public bool ButtonEnabled
    {
        get => buttonEnabled;
        set => SetProperty(ref buttonEnabled, value);
    }

    //添加按钮是否启用
    private bool addEnabled = true;

    public bool AddEnabled
    {
        get => addEnabled;
        set => SetProperty(ref addEnabled, value);
    }

    //状态指示
    private Visibility progressComplete = Visibility.Collapsed;

    public Visibility ProgressComplete
    {
        get => progressComplete;
        set => SetProperty(ref progressComplete, value);
    }

    //状态指示
    private Visibility progressError = Visibility.Collapsed;

    public Visibility ProgressError
    {
        get => progressError;
        set => SetProperty(ref progressError, value);
    }

    public AsyncRelayCommand OpenFileCommand
    {
        get;
    }

    private async Task OpenFile()
    {
        FileOpenPicker filePicker = new();

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(filePicker, hWnd);

        filePicker.FileTypeFilter.Add(".json");
        var selectedfiles = await filePicker.PickMultipleFilesAsync();
        if (selectedfiles != null)
        {
            ButtonEnabled = true;
            foreach (var selectedfile in selectedfiles)
            {
                if (ChartList.Any(item => item.Path == selectedfile.Path))
                {
                    await ShowDialog("Main_DialogWarningTitle", "Main_DialogWarningTextExists");
                    continue;
                }

                ChartList.Add(new ObservableChartListItem(
                    mainViewModel: this,
                    fileName: selectedfile.Name,
                    id: 1,
                    name: selectedfile.DisplayName,
                    difficulty: 1,
                    level: 1,
                    path: selectedfile.Path
                ));
            }
        }
    }

    public AsyncRelayCommand OpenFolderCommand
    {
        get;
    }

    private async Task OpenFolder()
    {
        FolderPicker folderPicker = new();

        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(App.MainWindow);
        WinRT.Interop.InitializeWithWindow.Initialize(folderPicker, hWnd);

        var selectedfolder = await folderPicker.PickSingleFolderAsync();
        if (selectedfolder != null)
        {
            OutputFolderPath = selectedfolder.Path;
        }
    }

    public RelayCommand StartConvertCommand
    {
        get;
    }

    /// <summary>
    /// 开始转换
    /// </summary>
    private async void StartConvert()
    {
        // 判断是否存在有效输入
        if (ChartList.Count < 1)
        {
            await ShowDialog("Main_DialogErrorTitle", "Main_DialogErrorMessageNoInput");
            return;
        }
        if (OutputFolderPath == null)
        {
            await ShowDialog("Main_DialogErrorTitle", "Main_DialogErrorMessageNoOutput");
            return;
        }

        //开始处理
        UpdateState("Ring");
        //循环处理文件
        for (int j = 0; j < ChartList.Count; j++)
        {
            SetProgress(ProgressStage.Read);
            ChartOrigin charto;
            try
            {
                var jsonString = _fileService.ReadString(ChartList[j].Path);
                jsonString = jsonString.Replace("\"$id\"", "id");
                jsonString = jsonString.Replace("\"$ref\"", "id");
                Debug.WriteLine(jsonString);
                charto = JsonConvert.DeserializeObject<ChartOrigin>(jsonString);
            }
            catch (Exception ex)
            {
                await ShowDialog("Main_DialogErrorTitle", $"读取操作错误：{ex.Message}");
                UpdateState("Error");
                return;
            }
            try
            {
                //转换 Drag
                SetProgress(ProgressStage.Drag);
                Task t = Task.Run(() =>
                {
                    for (int i = 0; i < charto.links.Count; i++) //循环读取links
                    {
                        Debug.WriteLine($"i:{i}");
                        for (int k = 0; k < charto.links[i].notes.Count; k++)//读取其中一个link，循环读取其中的notes
                        {
                            Debug.WriteLine($"k:{k}");
                            //识别Drag
                            charto.notes[charto.links[i].notes[k].id - 1].type = 1;
                        }
                    }
                });
                await t;
            }
            catch (Exception ex)
            {
                await ShowDialog("Main_DialogErrorTitle", $"转换 Drag 错误：{ex.Message}");
                UpdateState("Error");
                return;
            }
            //转换 Hold
            SetProgress(ProgressStage.Hold);
            try
            {
                var holdStartList = charto.notes.FindAll(item => Math.Abs(item.size - 1.5f) < 0.0001f);
                var holdEndList = charto.notes.FindAll(item => Math.Abs(item.size - 0.5f) < 0.0001f);
                Task t = Task.Run(() =>
                {
                    for (int i = 0; i < holdStartList.Count; i++)
                    {
                        //var tmpStart = charto.notes.Find(item => item.id == holdStartList[i].id).id;
                        charto.notes[holdStartList[i].id - 1].type = 2;
                        charto.notes[holdStartList[i].id - 1]._time = holdEndList[i].time - holdStartList[i].time;
                    }
                    for (int i = 0; i < holdEndList.Count; i++)
                    {
                        //var tmpStart = charto.notes.Find(item => item.id == holdStartList[i].id).id;
                        charto.notes.Remove(holdEndList[i]);
                    }
                });
                await t;
            }
            catch (Exception ex)
            {
                await ShowDialog("Main_DialogErrorTitle", $"转换 Hold 错误：{ex.Message}");
                UpdateState("Error");
                return;
            }
            //去除多余数据
            SetProgress(ProgressStage.Clean);
            try
            {
                for (int i = 0; i < charto.notes.Count; i++)
                {
                    if (charto.notes[i].type != 2)
                    {
                        charto.notes[i]._time = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowDialog("Main_DialogErrorTitle", $"清理操作错误：{ex.Message}");
                UpdateState("Error");
                return;
            }
            //开始保存
            SetProgress(ProgressStage.Save);
            //转换为新格式
            List<Stamp> arkStamps = new List<Stamp>();
            try
            {
                for (int i = 0; i < charto.notes.Count; i++)
                {
                    var idx = arkStamps.FindIndex(item => item.time == charto.notes[i].time);
                    if (idx == -1)
                    {
                        arkStamps.Add(new Stamp { time = charto.notes[i].time, notes = new List<Note>() });
                        arkStamps.Last().notes.Add(new Note
                        {
                            id = charto.notes[i].id,
                            type = charto.notes[i].type,
                            pos = charto.notes[i].pos,
                            holdTime = charto.notes[i]._time
                        });
                    }
                    else
                    {
                        arkStamps[idx].notes.Add(new Note
                        {
                            id = charto.notes[i].id,
                            type = charto.notes[i].type,
                            pos = charto.notes[i].pos,
                            holdTime = charto.notes[i]._time
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                await ShowDialog("Main_DialogErrorTitle", $"保存操作错误：{ex.Message}");
                UpdateState("Error");
                return;
            }
            ArkChart arkChart = new ArkChart
            {
                id = ChartList[j].ID,
                name = ChartList[j].Name,
                difficulty = ChartList[j].Difficulty,
                level = ChartList[j].Level,
                count = charto.notes.Count,
                stamps = arkStamps
            };
            //输出
            try
            {
                _fileService.Save(OutputFolderPath,
                    $"{ChartList[j].ID}.{ChartList[j].Name}.lv{ChartList[j].Level}.json", arkChart);
            }
            catch (Exception ex)
            {
                await ShowDialog("Main_DialogErrorTitle", ex.Message);
                UpdateState("Error");
                return;
            }
            //更新总进度
            ProgressValueTotal = RemapNumber(j / ChartList.Count, 0, 1, 0, 100);
        }
        ProgressValueTotal = 100;
        SetProgress(ProgressStage.Complete);
        UpdateState("Complete");
    }

    private float RemapNumber(float num, float oMin, float oMax, float tMin, float tMax)
    {
        var result = (tMax - tMin) / (oMax - oMin) * (num - oMin) + tMin;
        return result;
    }

    private void UpdateState(string state)
    {
        switch (state)
        {
            case "Ring":
                ProgressRingVisibility = Visibility.Visible;
                ButtonEnabled = false;
                AddEnabled = false;
                ProgressError = Visibility.Collapsed;
                ProgressComplete = Visibility.Collapsed;
                ProgressTextTotal = "Main_ProgressTextTotal".GetLocalized();
                break;

            case "Error":
                ProgressRingVisibility = Visibility.Collapsed;
                ButtonEnabled = true;
                AddEnabled = true;
                ProgressError = Visibility.Visible;
                ProgressComplete = Visibility.Collapsed;
                ProgressText = $"{ProgressText} {"Main_ProgressTextError".GetLocalized()}";
                ProgressTextTotal = "Main_ProgressTextTotal".GetLocalized();
                break;

            case "Complete":
                ProgressRingVisibility = Visibility.Collapsed;
                ButtonEnabled = true;
                AddEnabled = true;
                ProgressError = Visibility.Collapsed;
                ProgressComplete = Visibility.Visible;
                ProgressTextTotal = "Main_ProgressTextTotal".GetLocalized();
                break;

            default:
                ProgressRingVisibility = Visibility.Collapsed;
                ButtonEnabled = false;
                AddEnabled = true;
                ProgressError = Visibility.Collapsed;
                ProgressComplete = Visibility.Collapsed;
                ProgressTextTotal = null;
                break;
        }
    }

    private async Task<ContentDialogResult> ShowDialog(string title, string text)
    {
        ContentDialog dialog = new ContentDialog()
        {
            XamlRoot = App.MainWindow.Content.XamlRoot,
            Title = title.GetLocalized(),
            PrimaryButtonText = "Main_DialogErrorPrimaryButton".GetLocalized(),
            DefaultButton = ContentDialogButton.Primary
        };
        try
        {
            dialog.Content = text.GetLocalized();
        }
        catch
        {
            dialog.Content = text;
        }
        if (App.MainWindow.Content is FrameworkElement rootElement)
        {
            dialog.RequestedTheme = rootElement.RequestedTheme;
        }

        return await dialog.ShowAsync();
    }

    private void SetProgress(ProgressStage stage)
    {
        _progressService.SetStage(stage, out var tmpValue, out var tmpText);
        ProgressValue = tmpValue;
        ProgressText = tmpText;
    }

    public RelayCommand<object> ClearFileCommand
    {
        get;
    }

    private void ClearFile(object sender)
    {
        //ChartList.Clear();
        int count = ChartList.Count;
        for (int i = 0; i < count; i++)
        {
            ChartList.RemoveAt(0);
        }
        ButtonEnabled = false;
    }
}