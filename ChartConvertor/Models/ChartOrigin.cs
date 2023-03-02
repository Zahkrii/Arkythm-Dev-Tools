namespace ChartConvertor.Models;

public class ChartOrigin
{
    public int speed
    {
        get; set;
    }

    public List<NoteOrigin> notes
    {
        get; set;
    }

    public List<LinkOrigin> links
    {
        get; set;
    }
}

public class NoteOrigin
{
    public int id
    {
        get; set;
    }

    public int type
    {
        get; set;
    }

    public float pos
    {
        get; set;
    }

    public float size
    {
        get; set;
    }

    public float _time
    {
        get; set;
    }

    public int shift
    {
        get; set;
    }

    public float time
    {
        get; set;
    }
}

public class LinkOrigin
{
    public List<LinkNoteOrigin> notes
    {
        get; set;
    }
}

public class LinkNoteOrigin
{
    public int id
    {
        get; set;
    }
}