using ChartConvertor.Contracts.Services;
using ChartConvertor.Helpers;
using ProgressStage = ChartConvertor.Models.ProgressStageOptions.ProgressStage;

namespace ChartConvertor.Services;

public class ProgressService : IProgressService
{
    private readonly Dictionary<ProgressStage, ValueTuple<double, string>> _progressStage = new()
    {
        {ProgressStage.Default, (20,"Main_ProgressStage_Default".GetLocalized()) },
        {ProgressStage.Read, (10,"Main_ProgressStage_Read".GetLocalized()) },
        {ProgressStage.Drag, (20,"Main_ProgressStage_Drag".GetLocalized()) },
        {ProgressStage.Hold, (40,"Main_ProgressStage_Hold".GetLocalized()) },
        {ProgressStage.Clean, (60,"Main_ProgressStage_Clean".GetLocalized()) },
        {ProgressStage.Save, (80,"Main_ProgressStage_Save".GetLocalized()) },
        {ProgressStage.Complete, (100,"Main_ProgressStage_Complete".GetLocalized()) },
    };

    public void SetStage(ProgressStage stage, out double progressValue, out string progressText)
    {
        (double Value, string Text) tmp;
        switch (stage)
        {
            case ProgressStage.Read:
                _progressStage.TryGetValue(ProgressStage.Read, out tmp);
                progressValue = tmp.Value;
                progressText = tmp.Text;
                break;

            case ProgressStage.Drag:
                _progressStage.TryGetValue(ProgressStage.Drag, out tmp);
                progressValue = tmp.Value;
                progressText = tmp.Text;
                break;

            case ProgressStage.Hold:
                _progressStage.TryGetValue(ProgressStage.Hold, out tmp);
                progressValue = tmp.Value;
                progressText = tmp.Text;
                break;

            case ProgressStage.Clean:
                _progressStage.TryGetValue(ProgressStage.Clean, out tmp);
                progressValue = tmp.Value;
                progressText = tmp.Text;
                break;

            case ProgressStage.Save:
                _progressStage.TryGetValue(ProgressStage.Save, out tmp);
                progressValue = tmp.Value;
                progressText = tmp.Text;
                break;

            case ProgressStage.Complete:
                _progressStage.TryGetValue(ProgressStage.Complete, out tmp);
                progressValue = tmp.Value;
                progressText = tmp.Text;
                break;

            default:
                _progressStage.TryGetValue(ProgressStage.Read, out tmp);
                progressValue = tmp.Value;
                progressText = tmp.Text;
                break;
        }
    }
}