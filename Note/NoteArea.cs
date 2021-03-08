using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

namespace RhythmGameStarter
{
    public class NoteArea : MonoBehaviour
    {
        public UnityEvent OnInteractionBegin;
        public UnityEvent OnInteractionEnd;

        private TapEffectPool effect;

        public NoteEffect sustainEffect;

        private Track track;

        void Start()
        {
            track = GetComponentInParent<Track>();
            effect = GetComponentInParent<TapEffectPool>();
        }

        private List<Note> notesInRange = new List<Note>();

        private Note currentNote;

        private int currentFingerID = -1;

        private Vector2 touchDownPosition;

        private Vector3 touchDownNotePosition;

        private LongNoteDetecter longNoteDetecter;

        [HideInInspector]
        public KeyboardInputHandler keyboardInputHandler;

        [HideInInspector]
        public string key;

        void Update()
        {
            if (keyboardInputHandler)
            {
                if (Input.GetKeyDown(key))
                {
                    var fakeTouch = new Touch();
                    fakeTouch.phase = TouchPhase.Began;
                    TriggerNote(fakeTouch);
                }
                else if (Input.GetKeyUp(key))
                {
                    if (currentNote != null)
                        currentNote.inInteraction = false;

                    OnInteractionEnd.Invoke();
                }
            }

            var touch = Input.touches.Where(x => x.fingerId == currentFingerID).FirstOrDefault();

            if (currentFingerID != -1)
            {
                if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
                {
                    if (currentNote != null)
                        currentNote.inInteraction = false;
                        
                    OnInteractionEnd.Invoke();
                }
            }

            if (currentNote || (currentNote && keyboardInputHandler))
            {
                switch (currentNote.action)
                {
                    case Note.NoteAction.LongPress:
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary || (keyboardInputHandler && Input.GetKey(key)))
                        {
                            if (longNoteDetecter && longNoteDetecter.exitedLineArea)
                            {
                                // print("noteFinished");

                                AddCombo(currentNote, touchDownNotePosition);

                                longNoteDetecter.OnTouchUp();
                                longNoteDetecter = null;
                                currentNote = null;

                                sustainEffect.StopEffect();
                            }

                        }
                        else if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled || (keyboardInputHandler && Input.GetKeyUp(key)))
                        {
                            if (longNoteDetecter.exitedLineArea)
                            {
                                // print("noteFinished");
                                AddCombo(currentNote, touchDownNotePosition);
                            }
                            else
                            {
                                // print("noteFailed");
                                ComboSystem.INSTANCE.BreakCombo();
                            }

                            longNoteDetecter.OnTouchUp();
                            longNoteDetecter = null;
                            currentNote = null;

                            sustainEffect.StopEffect();
                        }
                        break;
                    case Note.NoteAction.Swipe:
                        if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Ended || keyboardInputHandler)
                        {
                            if (currentNote.alreadyMissed)
                            {
                                currentNote = null;
                                return;
                            }

                            bool addCombo = false;
                            if (keyboardInputHandler)
                            {
                                switch (currentNote.swipeDirection)
                                {
                                    case Note.SwipeDirection.Up:
                                        addCombo = Input.GetKey(keyboardInputHandler.up);
                                        break;
                                    case Note.SwipeDirection.Down:
                                        addCombo = Input.GetKey(keyboardInputHandler.down);
                                        break;
                                    case Note.SwipeDirection.Left:
                                        addCombo = Input.GetKey(keyboardInputHandler.left);
                                        break;
                                    case Note.SwipeDirection.Right:
                                        addCombo = Input.GetKey(keyboardInputHandler.right);
                                        break;
                                }
                            }

                            if (!addCombo && currentFingerID != -1 && touch.phase != TouchPhase.Began)
                            {
                                var main = Camera.main;
                                var diff = main.ScreenToWorldPoint(touchDownPosition) - main.ScreenToWorldPoint(touch.position);
                                // var diff = touchDownPosition - touch.position;
                                switch (currentNote.swipeDirection)
                                {
                                    case Note.SwipeDirection.Up:
                                        addCombo = diff.y >= currentNote.swipeThreshold;
                                        break;
                                    case Note.SwipeDirection.Down:
                                        addCombo = diff.y <= -currentNote.swipeThreshold;
                                        break;
                                    case Note.SwipeDirection.Left:
                                        addCombo = diff.x >= currentNote.swipeThreshold;
                                        break;
                                    case Note.SwipeDirection.Right:
                                        addCombo = diff.x <= -currentNote.swipeThreshold;
                                        break;
                                }
                                // print(currentNote.swipeDirection + " " + diff.x + " " + addCombo + " " + touch.position + " " + touch.fingerId + " " + touch.phase);
                            }

                            if (addCombo)
                            {
                                PlayHitSound(currentNote);
                                AddCombo(currentNote, touchDownNotePosition);

                                if (effect && !currentNote.noTapEffect)
                                    effect.EmitEffects(currentNote.transform);

                                KillNote(currentNote);
                                currentNote = null;
                            }
                        }
                        break;
                }
            }
        }

        public void ResetNoteArea()
        {
            currentFingerID = -1;
            notesInRange.Clear();
        }

        public void TriggerNote(Touch touch)
        {
            if (SongManager.INSTANCE.songPaused)
                return;

            currentFingerID = touch.fingerId;
            if (touch.phase == TouchPhase.Began)
            {
                OnInteractionBegin.Invoke();
            }

            var note = notesInRange.FirstOrDefault();
            if (note)
            {
                switch (note.action)
                {
                    case Note.NoteAction.Tap:
                        if (touch.phase != TouchPhase.Began) break;

                        if (effect && !note.noTapEffect)
                            effect.EmitEffects(note.transform);

                        PlayHitSound(note);
                        AddCombo(note, note.transform.position);

                        KillNote(note);

                        break;
                    case Note.NoteAction.LongPress:
                        if (touch.phase != TouchPhase.Began) break;

                        if (effect && !note.noTapEffect)
                            effect.EmitEffects(note.transform);

                        currentNote = note;
                        note.inInteraction = true;

                        touchDownNotePosition = note.transform.position;

                        longNoteDetecter = note.GetNoteDetecter();
                        longNoteDetecter.OnTouchDown();

                        PlayHitSound(note);

                        notesInRange.Remove(note);

                        sustainEffect.StartEffect(null);

                        break;
                    case Note.NoteAction.Swipe:
                        if (touch.phase != TouchPhase.Began) break;

                        notesInRange.Remove(note);

                        touchDownPosition = touch.position;
                        currentNote = note;
                        note.inInteraction = true;

                        touchDownNotePosition = note.transform.position;
                        break;
                }
            }
        }

        private void PlayHitSound(Note note)
        {
            if (!note.noHitSound)
            {
                track.trackHitAudio.clip = note.hitSound;
                track.trackHitAudio.Play();
            }
        }

        private void AddCombo(Note note, Vector3 touchDownPosition)
        {
            var noteLocalPositionInTrack = note.transform.parent.parent.InverseTransformPoint(touchDownPosition);
            var diff = track.lineArea.localPosition.y - noteLocalPositionInTrack.y;
            ComboSystem.INSTANCE.AddCombo(1, Mathf.Abs(diff), note.score);
        }

        private void KillNoteAnimation(Note note)
        {
            if (note.killAnim)
            {
                var anim = note.GetComponent<Animation>();
                anim.Play(note.killAnim.name, PlayMode.StopAll);
                note.transform.SetParent(null);

                if (TrackManager.INSTANCE.useNotePool)
                    StartCoroutine(DelayResetNote(note.gameObject, note.killAnim.length));
                else
                    Destroy(note.gameObject, note.killAnim.length);
            }
            else
            {
                if (TrackManager.INSTANCE.useNotePool)
                    TrackManager.INSTANCE.ResetNoteToPool(note.gameObject);
                else
                    Destroy(note.gameObject);
            }
        }

        IEnumerator DelayResetNote(GameObject note, float delay)
        {
            yield return new WaitForSeconds(delay);
            TrackManager.INSTANCE.ResetNoteToPool(note);
        }

        private void KillNote(Note note)
        {
            KillNoteAnimation(note);
            notesInRange.Remove(note);
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.tag == "Note")
            {
                notesInRange.Add(col.GetComponent<Note>());
            }
        }

        void OnTriggerExit(Collider col)
        {
            if (col.tag == "Note")
            {
                var outOfRange = col.GetComponent<Note>();
                notesInRange.Remove(outOfRange);

                if (currentNote != null)
                    if (currentNote == outOfRange)
                    {
                        currentNote = null;
                    }
            }
        }
    }
}