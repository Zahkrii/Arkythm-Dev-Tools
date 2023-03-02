using static ChartConvertor.Models.ProgressStageOptions;

namespace ChartConvertor.Contracts.Services;

public interface IProgressService
{
    void SetStage(ProgressStage stage, out double progressValue, out string progressText);
}