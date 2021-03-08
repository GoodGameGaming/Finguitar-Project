using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class TargetBoundary : MonoBehaviour
    {
        void OnTriggerExit(Collider col)
        {
            if (col.tag == "Note")
            {
                if (TrackManager.INSTANCE.useNotePool)
                {
                    TrackManager.INSTANCE.ResetNoteToPool(col.gameObject);
                }
                else
                {
                    Destroy(col.gameObject);
                }
            }
        }
    }
}