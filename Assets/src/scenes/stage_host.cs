using UnityEngine;

public class StageScene : MonoBehaviour
{
    public Transform stage__slot;

    public StageController stage_controller__ref;

    private Stage current_stage__ref;

    void __start__()
    {
        this.assignCurrentStageFromSlot();
    }

    void __update__()
    {
    }

    private void assignCurrentStageFromSlot()
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