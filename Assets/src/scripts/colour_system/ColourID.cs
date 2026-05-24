using UnityEngine;

public enum ColourID
{
    none,
    red,
    yellow,
    blue,
    purple,
    green,
    orange,
    /** Wrongly mixed colour. One more coluring to become inert. */
    mud,
    /** Inert colour. Cannot be mixed anymore. */
    inert,
}