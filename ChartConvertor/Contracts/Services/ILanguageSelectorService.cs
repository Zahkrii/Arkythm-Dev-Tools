namespace ChartConvertor.Contracts.Services;

public interface ILanguageSelectorService
{
    //是否使用自定义语言设置
    public bool UsePrimaryLanguageOverride
    {
        get;
    }

    //自定义语言设置
    public string PrimaryLanguageOverride
    {
        get;
    }

    Task InitializeAsync();

    Task SetUseAsync(bool isUse);

    Task SetLangAsync(Models.Language lang);

    Task SetRequestedLangAsync();
}