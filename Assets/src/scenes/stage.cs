using UnityEngine;

public class StageScene : MonoBehaviour
{
    public Transform stage__slot;

    public StageController stage_controller__ref;

    private Stage current_stage__ref;

    void __start__()
    {
        this.loadScene();
        this.stage_controller__ref.notifyStageCanSetup();
    }

    void __update__()
    {
    }

    /** Depends on whether the scene is loaded. E.g., from `GameFlowController` or already assigned scene. */
    private void loadScene()
    {
        if (this.stage__slot == null)
        {
            Debug.LogError("[StageScene] stage__slot is not assigned.");
            return;
        }

        if (this.stage_controller__ref == null)
        {
            Debug.LogError("[StageScene] stage_controller__ref is not assigned.");
            return;
        }

        // If there is scene to load, add to `this.stage__slot`.
        if (
            GameFlowController.instance != null &&
            GameFlowController.instance.current_stage_id != GameFlowController.id_not_set
        )
        {
            // Clear all the existing children in `this.stage__slot`.
            foreach (Transform child in this.stage__slot)
            {
                UnityEngine.Object.Destroy(child.gameObject);
            }

            // Add it as a child of `this.stage__slot`.
            if (!GameFlowController.instance.stage_prefab__dict.TryGetValue(GameFlowController.instance.current_stage_id, out var stage_prefab))
            {
                Debug.LogError($"[StageScene] Stage prefab was not found: {GameFlowController.instance.current_stage_id}");
                return;
            }
            UnityEngine.Object.Instantiate(stage_prefab, this.stage__slot);
        }

        // Try to see if there is a scene already in this stage scene.
        this.assignCurrentStageFromSlot();
    }

    private void assignCurrentStageFromSlot()
    {
        var stages = this.stage__slot.GetComponentsInChildren<Stage>(includeInactive: false);

        if (stages.Length == 0)
        {
            Debug.LogError("[StageScene] No Stage was found under stage__slot.");
            return;
        }

        if (stages.Length > 1)
        {
            Debug.LogError("[StageScene] Multiple Stages were found under stage__slot. Only the first one will be used.");
        }

        this.current_stage__ref = stages[0];

        this.stage_controller__ref.current_stage__ref = this.current_stage__ref;
    }

    void Start() { this.__start__(); }

    void Update() { this.__update__(); }
}