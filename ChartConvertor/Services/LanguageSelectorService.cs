using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ChartConvertor.Contracts.Services;
using ChartConvertor.Helpers;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls.Primitives;
using Windows.Globalization;

namespace ChartConvertor.Services;

public class LanguageSelectorService : ILanguageSelectorService
{
    private const string UseSettingsKey = "AppUseLanguageOverride";
    private const string LangSettingsKey = "AppLanguageOverride";

    private readonly ILocalSettingsService _localSettingsService;

    //是否使用自定义语言设置
    public bool UsePrimaryLanguageOverride
    {
        get; set;
    }

    //自定义语言设置
    public string PrimaryLanguageOverride
    {
        get; set;
    }

    public LanguageSelectorService(ILocalSettingsService localSettingsService)
    {
        _localSettingsService = localSettingsService;
    }

    public async Task InitializeAsync()
    {
        UsePrimaryLanguageOverride = await LoadUseFromSettingsAsync();
        PrimaryLanguageOverride = await LoadLangFromSettingsAsync();
        await Task.CompletedTask;
    }

    public async Task SetUseAsync(bool isUse)
    {
        UsePrimaryLanguageOverride = isUse;

        await SetRequestedLangAsync();
        await SaveUseInSettingsAsync(isUse);
    }

    public async Task SetLangAsync(Models.Language lang)
    {
        PrimaryLanguageOverride = lang.LanguageCode;

        await SetRequestedLangAsync();
        await SaveLangInSettingsAsync(lang);
    }

    public async Task SetRequestedLangAsync()
    {
        ApplicationLanguages.PrimaryLanguageOverride =
        UsePrimaryLanguageOverride ?
            PrimaryLanguageOverride :
            Windows.System.UserProfile.GlobalizationPreferences.Languages[0];

        await Task.CompletedTask;
    }

    private async Task<bool> LoadUseFromSettingsAsync()
    {
        var use = await _localSettingsService.ReadSettingAsync<bool>(UseSettingsKey);

        return use;
    }

    private async Task SaveUseInSettingsAsync(bool isUse)
    {
        await _localSettingsService.SaveSettingAsync(UseSettingsKey, isUse);
    }

    private async Task<string> LoadLangFromSettingsAsync()
    {
        var langName = await _localSettingsService.ReadSettingAsync<string>(LangSettingsKey);

        //Debug.WriteLine($"Load: {langName}");

        if (langName != null)
        {
            return langName;
        }

        return "en-US";
    }

    private async Task SaveLangInSettingsAsync(Models.Language lang)
    {
        await _localSettingsService.SaveSettingAsync(LangSettingsKey, lang.LanguageCode);
        //Debug.WriteLine($"Save: {lang.LanguageCode}");
    }
}