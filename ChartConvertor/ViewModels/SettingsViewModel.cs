using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Input;
using ChartConvertor.Contracts.Services;
using ChartConvertor.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Windows.ApplicationModel;
using ChartConvertor.Models;
using System.Diagnostics;

namespace ChartConvertor.ViewModels;

public class SettingsViewModel : ObservableRecipient
{
    private readonly IThemeSelectorService _themeSelectorService;
    private ElementTheme _elementTheme;
    private string _versionDescription;
    private readonly ILanguageSelectorService _languageSelectorService;
    private ObservableCollection<Language> _languages;
    private int _selectedLanguage = 0;
    private bool _isUseCustomLanguage = false;
    private bool _isLanguageChanged = false;

    public ElementTheme ElementTheme
    {
        get => _elementTheme;
        set => SetProperty(ref _elementTheme, value);
    }

    public string VersionDescription
    {
        get => _versionDescription;
        set => SetProperty(ref _versionDescription, value);
    }

    public ObservableCollection<Language> Languages
    {
        get => _languages;
        set => SetProperty(ref _languages, value);
    }

    public int SelectedLanguage
    {
        get => _selectedLanguage;
        set => SetProperty(ref _selectedLanguage, value);
    }

    public bool IsUseCustomLanguage
    {
        get => _isUseCustomLanguage;
        set
        {
            SetProperty(ref _isUseCustomLanguage, value);
            _languageSelectorService.SetUseAsync(value);
        }
    }

    public bool IsLanguageChanged
    {
        get => _isLanguageChanged;
        set => SetProperty(ref _isLanguageChanged, value);
    }

    public ICommand LanguageChangedCommand
    {
        get;
    }

    public ICommand SwitchThemeCommand
    {
        get;
    }

    public SettingsViewModel(IThemeSelectorService themeSelectorService, ILanguageSelectorService languageSelectorService)
    {
        _themeSelectorService = themeSelectorService;
        _elementTheme = _themeSelectorService.Theme;
        _versionDescription = GetVersionDescription();

        _languageSelectorService = languageSelectorService;

        Languages = new ObservableCollection<Language>
        {
            new Language { DisplayName = "English", LanguageCode = "en-US" },
            new Language { DisplayName = "简体中文", LanguageCode = "zh-CN" }
        };

        IsUseCustomLanguage = _languageSelectorService.UsePrimaryLanguageOverride;

        if (_languageSelectorService.PrimaryLanguageOverride == Languages[0].LanguageCode)
            SelectedLanguage = 0;
        else
            SelectedLanguage = 1;

        SwitchThemeCommand = new RelayCommand<ElementTheme>(
            async (param) =>
            {
                if (ElementTheme != param)
                {
                    ElementTheme = param;
                    await _themeSelectorService.SetThemeAsync(param);
                }
            });
        LanguageChangedCommand = new RelayCommand<int>(LanguageChanged);
    }

    private void LanguageChanged(int index)
    {
        IsLanguageChanged = true;
        _languageSelectorService.SetLangAsync(Languages[index]);
    }

    private static string GetVersionDescription()
    {
        Version version;

        if (RuntimeHelper.IsMSIX)
        {
            var packageVersion = Package.Current.Id.Version;

            version = new(packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
        }
        else
        {
            version = Assembly.GetExecutingAssembly().GetName().Version!;
        }

        return $"{"AppDisplayName".GetLocalized()} - {version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
    }
}