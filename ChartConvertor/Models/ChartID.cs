namespace ChartConvertor.Models;

/// <summary>
/// 曲目ID，后续增加请在这里增加ID
/// </summary>
public enum ChartID
{
    L1_1 = 0,
    L1_2,
    L1_3,
    L1_4,
    L1_5,
    L1_6,
    SE_1,
    SE_2,
    SE_3,
    SE_4,
    SE_5,
    EP_1
}

public class ChartIDItem
{
    public string displayName;
    public short chartID;
}