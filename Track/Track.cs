using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class Track : MonoBehaviour
    {
        public Transform lineArea;
        public NoteArea noteArea;

        [HideInInspector]
        public Transform notesParent;

        [HideInInspector]
        public IEnumerable<SongItem.MidiNote> allNotes;

        [HideInInspector]
        public AudioSource trackHitAudio;

        [HideInInspector]
        public List<Note> runtimeNote;

        void Awake()
        {
            trackHitAudio = GetComponent<AudioSource>();
            notesParent = new GameObject("Notes").transform;
            notesParent.SetParent(transform);
            ResetTrackPosition();
        }

        private void ResetTrackPosition()
        {
            notesParent.transform.position = lineArea.position;
            notesParent.transform.localEulerAngles = Vector3.zero;
        }

        public GameObject CreateNote(GameObject prefab)
        {
            var note = Instantiate(prefab);
            note.transform.SetParent(notesParent);
            note.transform.localEulerAngles = Vector3.zero;

            var noteScript = note.GetComponent<Note>();
            noteScript.inUse = true;
            runtimeNote.Add(noteScript);
            return note;
        }

        public void AttachNote(GameObject noteInstance)
        {
            noteInstance.transform.SetParent(notesParent);
            noteInstance.transform.localEulerAngles = Vector3.zero;

            runtimeNote.Add(noteInstance.GetComponent<Note>());
        }

        public void DestoryAllNotes()
        {
            runtimeNote.Clear();
            foreach (Transform child in notesParent)
            {
                GameObject.Destroy(child.gameObject);
            }
        }

        public void RecycleAllNotes(TrackManager manager)
        {
            runtimeNote.Clear();

            var currentNotes = new List<Transform>();
            foreach (Transform child in notesParent)
            {
                currentNotes.Add(child);
            }
            currentNotes.ForEach(x =>
            {
                manager.ResetNoteToPool(x.gameObject);
            });
        }

        public void ResetTrack()
        {
            runtimeNote.Clear();

            noteArea.ResetNoteArea();
        }
    }
}