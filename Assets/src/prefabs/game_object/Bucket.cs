using System;
using UnityEngine;

public class Bucket : MonoBehaviour
{
    // TODO: This should be simplified by `{ get; set; }` once Unity supports to C# 14.
    public ColourID contained_colour
    {
        get => this.stored__contained_colour;
        set
        {
            if (this.stored__contained_colour != value)
            {
                this.stored__contained_colour = value;
                this.is_colour_refresh_needed = true;
            }
        }
    }

    public SpriteRenderer body__renderer;

    [NonSerialized]
    public LevelPalette level_palette;

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
            this.body__renderer.color = this.level_palette.getColour(this.contained_colour);
            this.is_colour_refresh_needed = false;
        }
    }

    [SerializeField]
    private ColourID stored__contained_colour;

    void Start() { this.__start__(); }

    void Update() { this.__update__(); }
}
