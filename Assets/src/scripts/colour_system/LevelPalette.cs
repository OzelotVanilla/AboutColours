using System.Collections.Generic;
using UnityEngine;

/**
 * Converting colours defined in {@link ColourID} to Unity `Color`.
 */
public class StagePalette
{
    private Dictionary<ColourID, Color32> palette = new();

    public void setPaletteColour(ColourID colour_id, Color32 colour)
    {
        palette[colour_id] = colour;
    }

    public Color32 getColour(ColourID colour_id)
    {
        if (palette.TryGetValue(colour_id, out Color32 colour))
        {
            return colour;
        }

        return colour_id switch
        {
            ColourID.red => this.fallback__red,
            ColourID.yellow => this.fallback__yellow,
            ColourID.blue => this.fallback__blue,
            ColourID.purple => this.fallback__purple,
            ColourID.green => this.fallback__green,
            ColourID.orange => this.fallback__orange,
            ColourID.mud => this.fallback__mud,
            ColourID.inert => this.fallback__inert,
            _ => this.fallback__invalid, // default colour for none and unknown values
        };
    }

    protected readonly Color32 fallback__red = new(230, 0, 51, 255);

    protected readonly Color32 fallback__yellow = new(255, 217, 0, 255);

    protected readonly Color32 fallback__blue = new(0, 149, 217, 255);

    protected readonly Color32 fallback__purple = new(136, 72, 152, 255);

    protected readonly Color32 fallback__green = new(62, 179, 112, 255);

    protected readonly Color32 fallback__orange = new(238, 120, 0, 255);

    protected readonly Color32 fallback__mud = new(99, 92, 72, 255);

    protected readonly Color32 fallback__inert = new(0, 11, 0, 255);

    protected readonly Color32 fallback__invalid = new(255, 255, 255, 255);
}