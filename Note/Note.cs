using UnityEngine;

namespace RhythmGameStarter
{
    public class Note : MonoBehaviour
    {
        public int score;
        public NoteAction action;
        public SwipeDirection swipeDirection;
        public float swipeThreshold = 1f;
        public AnimationClip killAnim;
        public bool noTapEffect;
        public AudioClip hitSound;
        public bool noHitSound;
        public SpriteRenderer applyNoteLenghtTarget;
        public float noteLengthSizeOffset;

        #region HideInInspector
        [HideInInspector]
        public int noteType;
        [HideInInspector]
        public bool inUse;
        [HideInInspector]
        public bool inInteraction;
        [HideInInspector]
        public bool alreadyMissed;
        [HideInInspector]
        public float noteLength;
        [HideInInspector]
        public float noteTime;
        #endregion

        private BoxCollider target_collider;
        private BoxCollider m_collider;

        private Vector3[] initValues = new Vector3[5];


        public enum NoteAction
        {
            Tap, LongPress, Swipe
        }

        public enum SwipeDirection
        {
            Up, Down, Left, Right
        }

        void Awake()
        {
            if (applyNoteLenghtTarget)
                target_collider = applyNoteLenghtTarget.GetComponent<BoxCollider>();

            m_collider = GetComponent<BoxCollider>();
        }

        public void InitNoteLength(float length)
        {
            noteLength = length;
            if (applyNoteLenghtTarget)
            {
                initValues[0] = applyNoteLenghtTarget.size;
                initValues[1] = target_collider.center;
                initValues[2] = target_collider.size;
                initValues[3] = m_collider.center;
                initValues[4] = m_collider.size;

                //We set the size of the note
                var size = applyNoteLenghtTarget.size;
                size.y = length / SongManager.INSTANCE.secPerBeat * TrackManager.INSTANCE.beatSize;
                applyNoteLenghtTarget.size = size;

                //Update target collider
                var col_center = target_collider.center;
                var col_size = target_collider.size;

                col_center.y = size.y / 2 - noteLengthSizeOffset / 2;
                target_collider.center = col_center;

                col_size.y = size.y - noteLengthSizeOffset;
                target_collider.size = col_size;

                //Update self collider
                var col_center2 = m_collider.center;
                var col_size2 = m_collider.size;

                col_center2.y = size.y / 2 - col_size2.y / 2;
                m_collider.center = col_center2;

                col_size2.y = size.y;
                m_collider.size = col_size2;
            }
        }

        public void ResetForPool()
        {
            inUse = false;
            inInteraction = false;
            alreadyMissed = false;
            ResetNoteLength();
        }

        public void ResetNoteLength()
        {
            if (applyNoteLenghtTarget)
            {
                applyNoteLenghtTarget.size = initValues[0];
                target_collider.center = initValues[1];
                target_collider.size = initValues[2];
                m_collider.center = initValues[3];
                m_collider.size = initValues[4];

                applyNoteLenghtTarget.GetComponent<LongNoteDetecter>().exitedLineArea = false;
            }
        }

        public LongNoteDetecter GetNoteDetecter()
        {
            if (applyNoteLenghtTarget)
            {
                return applyNoteLenghtTarget.GetComponent<LongNoteDetecter>();
            }
            return null;
        }

        void OnTriggerExit(Collider col)
        {
            if (col.tag == "LineArea")
            {
                //The note is still under long press, dont break the combo
                if (action == NoteAction.LongPress && inInteraction)
                    return;

                alreadyMissed = true;
                ComboSystem.INSTANCE.BreakCombo();
            }
        }
    }
}