using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    // TODO: This should be simplified by `{ get; set; }` once Unity supports to C# 14.
    public ColourID paint_colour
    {
        get => this.stored__paint_colour;
        set
        {
            if (this.stored__paint_colour != value)
            {
                this.stored__paint_colour = value;
                this.is_colour_refresh_needed = true;
            }
        }
    }

    public SpriteRenderer body__renderer;

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
            this.body__renderer.color = this.level_palette.getColour(this.paint_colour);
            this.is_colour_refresh_needed = false;
        }
    }

    [SerializeField]
    private ColourID stored__paint_colour;

    void Start() { this.__start__(); }

    void Update() { this.__update__(); }
}
