using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class StageController : MonoBehaviour
{
    public Stage current_stage__ref;

    /** `{position: Tile}` */
    private Dictionary<Vector2Int, Tile> tile__dict = new();

    private Dictionary<Vector2Int, Wall> wall__dict = new();

    /** `{position: Bucket}` */
    private Dictionary<Vector2Int, Bucket> bucket__dict = new();

    private Player player__ref;

    private Vector2Int player_position = Vector2Int.zero;

    /** Move direction got from `InputController`. */
    private Vector2Int input_move_direction = Vector2Int.zero;

    /** Direction to move and consume in next game tick. */
    private Vector2Int queued_move_direction = Vector2Int.zero;

    public float step_interval__secs = 0.4f;

    private bool is_step_cooling_down = false;

    void __setup__()
    {
        this.assignRefs();
    }

    void __update__() { }

    #region Step handling

    void tryConsumeQueuedStep()
    {
        if (this.is_step_cooling_down) { return; }

        if (this.queued_move_direction == Vector2Int.zero) { return; }

        var direction = this.queued_move_direction;
        this.queued_move_direction = Vector2Int.zero;

        this.consumeStep(direction);
    }

    void consumeStep(Vector2Int direction)
    {
        // Consume.
        this.tryMovePlayer(direction);
        if (this.is_stage_clear)
        {
            Debug.Log("Stage clear!");
        }

        // Publish new.
        StageStepBus.publishStep();

        // Wait.
        this.StartCoroutine(this.cooldownThenTryNextStep());
    }

    System.Collections.IEnumerator cooldownThenTryNextStep()
    {
        this.is_step_cooling_down = true;

        yield return new WaitForSeconds(this.step_interval__secs);

        this.is_step_cooling_down = false;

        // In case that user is keeping holding and `setInputMoveDirection` is not called. 
        if (this.input_move_direction != Vector2Int.zero)
        {
            this.queued_move_direction = this.input_move_direction;
            this.tryConsumeQueuedStep();
        }
    }

    #endregion

    void __onEnable__()
    {
    }

    void __onDisable__()
    {
    }

    /**
     * Consume `queued_move_direction` and try to move player.
     */
    void tryMovePlayer(Vector2Int direction)
    {
        var target_position = this.player_position + direction;

        // Change the facing by rotation.
        this.updatePlayerFacing(direction);

        // Check if able to move.
        if (this.wall__dict.ContainsKey(target_position))
        {
            return;
        }

        // Move player.
        this.player__ref.transform.localPosition += new Vector3(direction.x, direction.y, 0);
        this.player_position = target_position;
        this.resolvePlayerSuccessfulMove();
    }

    /** See if there is something that can be interacted with newly-moved player. */
    void resolvePlayerSuccessfulMove()
    {
        // Check if there is a bucket.
        if (this.bucket__dict.TryGetValue(this.player_position, out Bucket bucket))
        {
            this.player__ref.paint_colour = bucket.contained_colour;
        }

        // Check if there is a tile.
        if (this.tile__dict.TryGetValue(this.player_position, out Tile tile))
        {
            tile.current_colour = ColourMixer.mix(tile.current_colour, this.player__ref.paint_colour);
        }
    }

    void updatePlayerFacing(Vector2Int direction)
    {
        var facing_angle = (Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg) - 90f;
        this.player__ref.transform.rotation = Quaternion.Euler(0, 0, facing_angle);
    }

    public void setInputMoveDirection(Vector2Int direction)
    {
        this.input_move_direction = direction;

        if (direction == Vector2Int.zero)
        {
            return;
        }

        this.queued_move_direction = direction;

        this.tryConsumeQueuedStep();
    }

    public bool is_stage_clear
    {
        get
        {
            foreach (var tile in this.tile__dict.Values)
            {
                if (tile.target_colour != ColourID.none && tile.current_colour != tile.target_colour)
                {
                    return false;
                }
            }

            return true;
        }
    }


    public void requestUndo()
    {

    }

    public void requestRestart()
    {
        var current_scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current_scene.name);
    }

    public void requestOpenMenu()
    {

    }

    public void assignRefs()
    {
        this.tile__dict.Clear();
        this.bucket__dict.Clear();
        this.player__ref = null;

        if (this.current_stage__ref == null)
        {
            Debug.LogError("[StageController] current_stage__ref is not assigned.");
            return;
        }

        // Will throw if reference is missing.
        this.current_stage__ref.checkInspectorReference();

        this.collectTiles();
        this.collectWalls();
        this.collectBuckets();
        this.collectPlayer();
    }

    Vector2Int getGridPositionFromTransform(Transform transform)
    {
        var position = transform.localPosition;

        return new Vector2Int(
            Mathf.RoundToInt(position.x),
            Mathf.RoundToInt(position.y)
        );
    }

    private void collectTiles()
    {
        var tiles = this.current_stage__ref.tile__container__ref.GetComponentsInChildren<Tile>();

        foreach (var tile in tiles)
        {
            var position = this.getGridPositionFromTransform(tile.transform);

            if (this.tile__dict.ContainsKey(position))
            {
                Debug.LogError($"[StageController] duplicated tile position: {position}");
                continue;
            }

            this.tile__dict.Add(position, tile);
        }
    }

    private void collectWalls()
    {
        var walls = this.current_stage__ref.wall__container__ref.GetComponentsInChildren<Wall>();

        foreach (var wall in walls)
        {
            var position = this.getGridPositionFromTransform(wall.transform);

            if (this.wall__dict.ContainsKey(position))
            {
                Debug.LogError($"[StageController] duplicated wall position: {position}");
                continue;
            }

            this.wall__dict.Add(position, wall);
        }
    }

    private void collectBuckets()
    {
        var buckets = this.current_stage__ref.object__container__ref.GetComponentsInChildren<Bucket>();

        foreach (var bucket in buckets)
        {
            var position = this.getGridPositionFromTransform(bucket.transform);

            if (this.bucket__dict.ContainsKey(position))
            {
                Debug.LogError($"[StageController] duplicated bucket position: {position}");
                continue;
            }

            this.bucket__dict.Add(position, bucket);
        }
    }

    private void collectPlayer()
    {
        var players = this.current_stage__ref.player__container__ref.GetComponentsInChildren<Player>();

        if (players.Length == 0)
        {
            Debug.LogError("[StageController] Player is not found.");
            return;
        }

        if (players.Length > 1)
        {
            Debug.LogError("[StageController] Multiple players found. Only first one will be used.");
        }

        this.player__ref = players[0];
        this.player_position = this.getGridPositionFromTransform(this.player__ref.transform);
    }

    public void notifyStageCanSetup() { this.__setup__(); }

    void Update() { this.__update__(); }

    void OnEnable() { this.__onEnable__(); }

    void OnDisable() { this.__onDisable__(); }
}