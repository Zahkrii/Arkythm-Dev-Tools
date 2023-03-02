namespace ChartConvertor.Models;

public class ArkChart
{
    public short id;
    public string name;
    public short difficulty;
    public short level;
    public int count;
    public List<Stamp> stamps;
}

public class Stamp
{
    public float time;
    public List<Note> notes;
}

public class Note
{
    public int id;
    public int type;
    public float pos;
    public float holdTime;
}