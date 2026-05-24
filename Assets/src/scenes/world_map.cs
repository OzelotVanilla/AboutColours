using UnityEngine;

public class WorldMapScene : MonoBehaviour
{
    public WorldMapData world_map_data__ref;

    void __start__()
    {
        if (this.world_map_data__ref == null)
        {
            Debug.LogError("[WorldMapScene] world_map_data__ref is not assigned.");
            return;
        }

        // this.world_map_data__ref.readStationAndMetroLines();
    }

    void __update__()
    {
    }

    void Start() { this.__start__(); }

    void Update() { this.__update__(); }
}