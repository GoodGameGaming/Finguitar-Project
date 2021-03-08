using UnityEngine;
using System.Linq;

namespace RhythmGameStarter
{
    public class TrackTriggerArea : MonoBehaviour
    {
        public TouchEvent OnNoteTrigger;

        public void TriggerNote(Touch touch)
        {
            OnNoteTrigger.Invoke(touch);
        }
    }
}