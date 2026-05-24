using UnityEngine;

[System.Serializable]
public class StationData
{
    public string station_id;
    public string display_name;
    public Vector2 position;

    // Stored in station json.
    public string[] stage_ids;

    public string[] unlocks_after_clear;

    // Runtime-resolved prefab-era cache.
    [System.NonSerialized]
    public Stage[] stage_prefabs;
}