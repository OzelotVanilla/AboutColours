using System.Collections.Generic;
using UnityEngine;

/**
 * This class is aimed to control the game flow,
 *  such as loading stages.
 */
public class GameFlowController : MonoBehaviour
{
    public static string id_not_set = "<not_setted_yet>";

    public static GameFlowController instance;

    public Stage[] stage_prefab__arr;

    /** Generated from `stage_prefab__arr` when awake-ed. */
    public Dictionary<string, Stage> stage_prefab__dict;

    public string current_stage_id = GameFlowController.id_not_set;

    void __awake__()
    {
        if (GameFlowController.instance != null && GameFlowController.instance != this)
        {
            UnityEngine.Object.Destroy(this.gameObject);
            return;
        }

        GameFlowController.instance = this;
        UnityEngine.Object.DontDestroyOnLoad(this.gameObject);

        this.buildStagePrefabDict();
    }

    public void requestMoveToStage(string stage_id)
    {
        this.current_stage_id = stage_id;
        UnityEngine.SceneManagement.SceneManager.LoadScene("stage");
    }

    public void requestUnloadCurrentStage()
    {
        this.current_stage_id = GameFlowController.id_not_set;
        UnityEngine.SceneManagement.SceneManager.LoadScene("world_map");
    }

    void buildStagePrefabDict()
    {
        this.stage_prefab__dict = new Dictionary<string, Stage>();

        if (this.stage_prefab__arr == null)
        {
            Debug.LogError("[GameFlowController] stage_prefab__arr is not assigned.");
            return;
        }

        // Check for duplicate stage ids.
        foreach (var stage in this.stage_prefab__arr)
        {
            if (this.stage_prefab__dict.ContainsKey(stage.stage_id))
            {
                Debug.LogError($"[GameFlowController] Duplicate stage id found: {stage.stage_id}");
                continue;
            }

            this.stage_prefab__dict[stage.stage_id] = stage;
        }
    }

    void Awake() { this.__awake__(); }
}