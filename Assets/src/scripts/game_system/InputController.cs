using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public InputActionAsset input_actions__asset;

    public StageController stage_controller__ref;

    void __start__() { }

    void __update__() { }

    void __awake__()
    {
        this.checkInspectorReference();
        this.assignRefToAction();
    }

    void __onEnable__()
    {
        this.subscribeActionCallbacks();
        this.stage_action_map__ref.Enable();
    }

    void __onDisable__()
    {
        this.unsubscribeActionCallbacks();
        this.stage_action_map__ref.Disable();
    }

    private Vector2Int previous_inputted_direction = Vector2Int.zero;

    private Vector2Int previous_intended_direction = Vector2Int.zero;

    /**
     * Get the `direction` by the algorithm below:
     * * If `previous_inputted_direction == inputted_direction`, do nothing.
     * * If the `previous_inputted_direction != inputted_direction` is zero, set `inputted_direction` to input direction.
     *   * Additionally, if `inputted_direction` is diagonal, compare to `previous_inputted_direction`.
     *     Choose the newly input direction.
     *     E.g., if previous is right and new input is up-right, then choose up;
     *      if previous is up and new input is up-right, then choose right.
     */
    void on__action_performed__move(InputAction.CallbackContext context)
    {
        var raw_input_vector = context.ReadValue<Vector2>();
        var inputted_direction = Vector2Int.RoundToInt(raw_input_vector);
        var intended_direction = inputted_direction;

        if (inputted_direction == this.previous_inputted_direction) { return; }

        // Check if diagonal input.
        if (inputted_direction.x != 0 && inputted_direction.y != 0)
        {
            // If previous is zero, then prefer the horizontal input.
            if (this.previous_inputted_direction == Vector2Int.zero)
            {
                intended_direction = new Vector2Int(inputted_direction.x, 0);
            }
            // If previous is not diagonal and not zero, then choose the difference.
            else if (this.previous_inputted_direction.x == 0 || this.previous_inputted_direction.y == 0)
            {
                intended_direction = inputted_direction - this.previous_inputted_direction;
            }
            // Else, although rare, let the inverted previous intended direction be the intended direction.
            else
            {
                intended_direction = -this.previous_intended_direction;
            }
        }

        // For final check, `intended_direction`'s length should not be greater than 1.
        if (intended_direction.sqrMagnitude > 1)
        {
            Debug.LogWarning($"InputController: The intended direction {intended_direction} has a magnitude greater than 1. Normalising it.");
            intended_direction = Vector2Int.RoundToInt(new Vector2(intended_direction.x, intended_direction.y).normalized);
        }

        this.previous_inputted_direction = inputted_direction;
        this.previous_intended_direction = intended_direction;
        this.stage_controller__ref.setInputMoveDirection(intended_direction);
    }

    void on__action_canceled__move(InputAction.CallbackContext context)
    {
        this.previous_inputted_direction = Vector2Int.zero;
        this.previous_intended_direction = Vector2Int.zero;
        this.stage_controller__ref.setInputMoveDirection(Vector2Int.zero);
    }

    void on__action_performed__restart(InputAction.CallbackContext context)
    {
        this.stage_controller__ref.requestRestart();
    }

    void on__action_performed__undo(InputAction.CallbackContext context)
    {
        this.stage_controller__ref.requestUndo();
    }

    void on__action_performed__menu(InputAction.CallbackContext context)
    {
        this.stage_controller__ref.requestOpenMenu();
    }

    void assignRefToAction()
    {
        this.stage_action_map__ref = this.input_actions__asset.FindActionMap("stage", throwIfNotFound: true);
        this.move_action__ref = this.input_actions__asset.FindAction("stage/move", throwIfNotFound: true);
        this.restart_action__ref = this.input_actions__asset.FindAction("stage/restart", throwIfNotFound: true);
        this.undo_action__ref = this.input_actions__asset.FindAction("stage/undo", throwIfNotFound: true);
        this.menu_action__ref = this.input_actions__asset.FindAction("stage/menu", throwIfNotFound: true);
    }

    void subscribeActionCallbacks()
    {
        this.move_action__ref.performed += this.on__action_performed__move;
        this.move_action__ref.canceled += this.on__action_canceled__move;
        this.restart_action__ref.performed += this.on__action_performed__restart;
        this.undo_action__ref.performed += this.on__action_performed__undo;
        this.menu_action__ref.performed += this.on__action_performed__menu;
    }

    void unsubscribeActionCallbacks()
    {
        this.move_action__ref.performed -= this.on__action_performed__move;
        this.move_action__ref.canceled -= this.on__action_canceled__move;
        this.restart_action__ref.performed -= this.on__action_performed__restart;
        this.undo_action__ref.performed -= this.on__action_performed__undo;
        this.menu_action__ref.performed -= this.on__action_performed__menu;
    }

    void checkInspectorReference()
    {
        if (this.input_actions__asset == null)
        {
            throw new System.Exception("InputController: `input_actions__asset` is not assigned in the inspector.");
        }
        if (this.stage_controller__ref == null)
        {
            throw new System.Exception("InputController: `stage_controller__ref` is not assigned in the inspector.");
        }
    }

    private InputActionMap stage_action_map__ref;

    private InputAction move_action__ref;

    private InputAction restart_action__ref;

    private InputAction undo_action__ref;

    private InputAction menu_action__ref;

    void Start() { this.__start__(); }

    void Update() { this.__update__(); }

    void Awake() { this.__awake__(); }

    void OnEnable() { this.__onEnable__(); }

    void OnDisable() { this.__onDisable__(); }
}
