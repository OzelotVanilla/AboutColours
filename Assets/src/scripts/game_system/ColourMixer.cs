public static class ColourMixer
{
    public static ColourID mix(ColourID base_colour, ColourID added_colour)
    {
        if (base_colour == ColourID.inert)
        {
            return base_colour;
        }

        if (base_colour == ColourID.mud && added_colour != ColourID.none)
        {
            return ColourID.inert;
        }

        if (added_colour == ColourID.none)
        {
            return base_colour;
        }

        if (base_colour == ColourID.none)
        {
            return added_colour;
        }

        if (base_colour == added_colour)
        {
            return base_colour;
        }

        if (
            (base_colour == ColourID.red && added_colour == ColourID.yellow)
            || (base_colour == ColourID.yellow && added_colour == ColourID.red)
        )
        {
            return ColourID.orange;
        }

        if (
            (base_colour == ColourID.yellow && added_colour == ColourID.blue)
            || (base_colour == ColourID.blue && added_colour == ColourID.yellow)
        )
        {
            return ColourID.green;
        }

        if (
            (base_colour == ColourID.blue && added_colour == ColourID.red)
            || (base_colour == ColourID.red && added_colour == ColourID.blue)
        )
        {
            return ColourID.purple;
        }

        // Wrongly mixed colour results in mud.
        return ColourID.mud;
    }
}