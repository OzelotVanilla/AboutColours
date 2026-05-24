using UnityEngine;

public class Stage : MonoBehaviour
{
    public Transform tile__container__ref;

    public Transform object__container__ref;

    public Transform player__container__ref;

    public void checkInspectorReference()
    {
        if (this.tile__container__ref == null)
        {
            throw new System.Exception("Stage: tile__container__ref is not assigned in inspector.");
        }

        if (this.object__container__ref == null)
        {
            throw new System.Exception("Stage: object__container__ref is not assigned in inspector.");
        }

        if (this.player__container__ref == null)
        {
            throw new System.Exception("Stage: player__container__ref is not assigned in inspector.");
        }
    }
}
