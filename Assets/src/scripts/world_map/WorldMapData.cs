using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

/// <summary>
/// Runtime world map data loader.
/// Loads station json, metro line json, and stage prefabs via Addressables labels.
/// </summary>
public class WorldMapData : MonoBehaviour
{
    [Header("Addressables labels")]
    [SerializeField] private string station_label = "level_station";
    [SerializeField] private string metro_line_label = "level_metro_line";
    [SerializeField] private string stage_prefab_label = "level_stage_prefab";

    [System.NonSerialized]
    public Dictionary<string, StationData> station__dict = new();

    [System.NonSerialized]
    public Dictionary<string, MetroLineData> metro_line__dict = new();

    [System.NonSerialized]
    public Dictionary<string, Stage> stage__dict = new();

    private bool is_loaded = false;

    private AsyncOperationHandle<IList<TextAsset>> station_files__handle;
    private AsyncOperationHandle<IList<TextAsset>> metro_line_files__handle;
    private AsyncOperationHandle<IList<GameObject>> stage_prefabs__handle;

    private bool station_files__loaded = false;
    private bool metro_line_files__loaded = false;
    private bool stage_prefabs__loaded = false;

    public async Task readAllAsync()
    {
        if (this.is_loaded)
        {
            return;
        }

        this.station__dict.Clear();
        this.metro_line__dict.Clear();
        this.stage__dict.Clear();

        await this.readStationsAsync();
        await this.readMetroLinesAsync();
        await this.readStagePrefabsAsync();

        this.resolveStationStagePrefabs();
        this.generateFallbackStationsFromUnreferencedStages();

        this.is_loaded = true;
    }

    private async Task readStationsAsync()
    {
        this.station_files__handle = Addressables.LoadAssetsAsync<TextAsset>(this.station_label, null);

        await this.station_files__handle.Task;
        this.station_files__loaded = true;

        if (this.station_files__handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[WorldMapData] Failed to load station files by label: {this.station_label}");
            return;
        }

        foreach (var station_file in this.station_files__handle.Result)
        {
            var station_data = this.deserialiseFromStationFile(station_file);

            if (station_data == null)
            {
                Debug.LogError($"[WorldMapData] Failed to parse station file: {station_file.name}");
                continue;
            }

            if (string.IsNullOrWhiteSpace(station_data.station_id))
            {
                Debug.LogError($"[WorldMapData] Empty station_id in file: {station_file.name}");
                continue;
            }

            if (this.station__dict.ContainsKey(station_data.station_id))
            {
                Debug.LogError(
                    $"[WorldMapData] Duplicate station_id: {station_data.station_id} in file {station_file.name}."
                );
                continue;
            }

            this.station__dict[station_data.station_id] = station_data;
        }
    }

    private async Task readMetroLinesAsync()
    {
        this.metro_line_files__handle =
            Addressables.LoadAssetsAsync<TextAsset>(this.metro_line_label, null);

        await this.metro_line_files__handle.Task;
        this.metro_line_files__loaded = true;

        if (this.metro_line_files__handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[WorldMapData] Failed to load metro line files by label: {this.metro_line_label}");
            return;
        }

        foreach (var metro_line_file in this.metro_line_files__handle.Result)
        {
            var metro_line_data = this.deserialiseFromMetroLineFile(metro_line_file);

            if (metro_line_data == null)
            {
                Debug.LogError($"[WorldMapData] Failed to parse metro line file: {metro_line_file.name}");
                continue;
            }

            if (string.IsNullOrWhiteSpace(metro_line_data.metro_line_id))
            {
                Debug.LogError($"[WorldMapData] Empty metro_line_id in file: {metro_line_file.name}");
                continue;
            }

            if (this.metro_line__dict.ContainsKey(metro_line_data.metro_line_id))
            {
                Debug.LogError(
                    $"[WorldMapData] Duplicate metro_line_id: {metro_line_data.metro_line_id} in file {metro_line_file.name}."
                );
                continue;
            }

            this.metro_line__dict[metro_line_data.metro_line_id] = metro_line_data;
        }
    }

    private async Task readStagePrefabsAsync()
    {
        this.stage_prefabs__handle =
            Addressables.LoadAssetsAsync<GameObject>(this.stage_prefab_label, null);

        await this.stage_prefabs__handle.Task;
        this.stage_prefabs__loaded = true;

        if (this.stage_prefabs__handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError($"[WorldMapData] Failed to load stage prefabs by label: {this.stage_prefab_label}");
            return;
        }

        foreach (var prefab_object in this.stage_prefabs__handle.Result)
        {
            var stage = prefab_object.GetComponent<Stage>();

            if (stage == null)
            {
                Debug.LogError($"[WorldMapData] Prefab has no Stage component: {prefab_object.name}");
                continue;
            }

            if (string.IsNullOrWhiteSpace(stage.stage_id))
            {
                Debug.LogError($"[WorldMapData] Stage prefab has empty stage_id: {prefab_object.name}");
                continue;
            }

            if (this.stage__dict.ContainsKey(stage.stage_id))
            {
                Debug.LogError($"[WorldMapData] Duplicate stage_id: {stage.stage_id} from prefab {prefab_object.name}");
                continue;
            }

            this.stage__dict[stage.stage_id] = stage;
        }
    }

    private void resolveStationStagePrefabs()
    {
        foreach (var station in this.station__dict.Values)
        {
            if (station.stage_ids == null || station.stage_ids.Length == 0)
            {
                station.stage_prefabs = new Stage[0];
                Debug.LogWarning($"[WorldMapData] Station has no stage_ids: {station.station_id}");
                continue;
            }

            var resolved_stage_prefabs = new List<Stage>();

            foreach (var stage_id in station.stage_ids)
            {
                if (string.IsNullOrWhiteSpace(stage_id))
                {
                    Debug.LogError($"[WorldMapData] Station has empty stage_id: {station.station_id}");
                    continue;
                }

                if (!this.stage__dict.TryGetValue(stage_id, out var stage_prefab))
                {
                    Debug.LogError($"[WorldMapData] Station references unknown stage_id: {station.station_id} -> {stage_id}");
                    continue;
                }

                resolved_stage_prefabs.Add(stage_prefab);
            }

            station.stage_prefabs = resolved_stage_prefabs.ToArray();
        }
    }

    private void generateFallbackStationsFromUnreferencedStages()
    {
        var referenced_stage_ids = new HashSet<string>();

        foreach (var station in this.station__dict.Values)
        {
            if (station.stage_ids == null)
            {
                continue;
            }

            foreach (var stage_id in station.stage_ids)
            {
                if (!string.IsNullOrWhiteSpace(stage_id))
                {
                    referenced_stage_ids.Add(stage_id);
                }
            }
        }

        int fallback_index = 0;

        foreach (var pair in this.stage__dict)
        {
            var stage_id = pair.Key;
            var stage_prefab = pair.Value;

            if (referenced_stage_ids.Contains(stage_id))
            {
                continue;
            }

            var station_id = $"station__fallback__{stage_id}";

            if (this.station__dict.ContainsKey(station_id))
            {
                Debug.LogError($"[WorldMapData] Fallback station_id conflicts: {station_id}");
                continue;
            }

            var station = new StationData
            {
                station_id = station_id,
                display_name = stage_id,
                position = this.makeFallbackPosition(fallback_index),
                stage_ids = new[] { stage_id },
                stage_prefabs = new[] { stage_prefab },
                unlocks_after_clear = new string[0],
            };

            this.station__dict[station.station_id] = station;
            fallback_index += 1;

            Debug.Log($"[WorldMapData] Generated fallback station for stage: {stage_id}");
        }
    }

    private Vector2 makeFallbackPosition(int index)
    {
        const float spacing = 2.5f;
        return new Vector2(index * spacing, 0.0f);
    }

    public StationData deserialiseFromStationFile(TextAsset station_file)
    {
        return JsonUtility.FromJson<StationData>(station_file.text);
    }

    public MetroLineData deserialiseFromMetroLineFile(TextAsset metro_line_file)
    {
        return JsonUtility.FromJson<MetroLineData>(metro_line_file.text);
    }

    public bool tryGetStagePrefab(string stage_id, out Stage stage_prefab)
    {
        return this.stage__dict.TryGetValue(stage_id, out stage_prefab);
    }

    private void OnDestroy()
    {
        this.releaseAddressableHandles();
    }

    private void releaseAddressableHandles()
    {
        if (this.station_files__loaded)
        {
            Addressables.Release(this.station_files__handle);
            this.station_files__loaded = false;
        }

        if (this.metro_line_files__loaded)
        {
            Addressables.Release(this.metro_line_files__handle);
            this.metro_line_files__loaded = false;
        }

        if (this.stage_prefabs__loaded)
        {
            Addressables.Release(this.stage_prefabs__handle);
            this.stage_prefabs__loaded = false;
        }
    }
}