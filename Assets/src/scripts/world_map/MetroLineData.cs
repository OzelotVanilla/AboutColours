using UnityEngine;

[System.Serializable]
public class MetroLineData
{
    public string metro_line_id;
    public string display_name;
    public Color color;

    // Ordered station ids on this line.
    public string[] station_ids;
}