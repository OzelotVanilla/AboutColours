using System;
using UnityEngine;

public class Tile : MonoBehaviour
{
    // TODO: This should be simplified by `{ get; set; }` once Unity supports to C# 14.
    public ColourID target_colour
    {
        get => this.stored__target_colour;
        set
        {
            if (this.stored__target_colour != value)
            {
                this.stored__target_colour = value;
                this.is_colour_refresh_needed = true;
            }
        }
    }

    // TODO: This should be simplified by `{ get; set; }` once Unity supports to C# 14.
    public ColourID current_colour
    {
        get => this.stored__current_colour;
        set
        {
            this.stored__current_colour = value;
            this.is_colour_refresh_needed = true;
        }
    }

    public SpriteRenderer outer_frame__renderer;

    public SpriteRenderer inner_square__renderer;

    [NonSerialized]
    public StagePalette level_palette;

    private bool is_colour_refresh_needed = true;

    private void __start__()
    {
        this.level_palette ??= new();
        this.is_colour_refresh_needed = true;
    }

    private void __update__()
    {
        if (this.is_colour_refresh_needed)
        {
            this.outer_frame__renderer.color = this.level_palette.getColour(this.target_colour);
            this.inner_square__renderer.color = this.level_palette.getColour(this.current_colour);
            this.is_colour_refresh_needed = false;
        }
    }

    [SerializeField]
    private ColourID stored__target_colour;

    [SerializeField]
    private ColourID stored__current_colour;

    void Start() { this.__start__(); }

    void Update() { this.__update__(); }
}