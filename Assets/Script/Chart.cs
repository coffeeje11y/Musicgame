using System.Collections.Generic;

[System.Serializable]
public class NoteData
{
    public int LPB;
    public int num;
    public int block;
    public int type;
    public List<NoteData> notes;
}

[System.Serializable]
public class Chart
{
    public string name;
    public int maxBlock;
    public float BPM;
    public float offset;
    public List<NoteData> notes;
}
