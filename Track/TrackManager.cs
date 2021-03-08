using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace RhythmGameStarter
{
    public class TrackManager : MonoBehaviour
    {
        public static TrackManager INSTANCE;

        [Header("[Mapping]")]
        public List<MidiTrackMapping> midiTracksMappingList;
        public NotePrefabMapping notePrefabMapping;
        public int midiId=0;

        [Header("[Properties]")]
        [Space]
        [Tooltip("Offset in position local space, useful for extra audio latency tuning in game")]
        public float hitOffset;
        [Tooltip("The spacing for each note, higher the value will cause the note speed faster, by using IndividualNote SyncMode, you can change this in realtime")]
        public float beatSize;
        [Tooltip("If using notePool, the note object will be recycled instead of created and destroyed in runtime")]
        public bool useNotePool = true;
        public float poolLookAheadTime = 5;
        [Tooltip("Two different mode (Track, IndividualNote) for synchronizing the node postion with music")]
        public SyncMode syncMode;
        [Tooltip("Applying extra smoothing with Time.deltaTime in Track syncMode, and using Vector3.Lerp in IndividualNote syncMode, the smoothing could be insignificant under some situration")]
        public bool syncSmoothing;

        private Track[] tracks;

        private SongManager songManager;

        private Transform notesPoolParent;

        private List<Note> pooledNotes = new List<Note>();

        public enum SyncMode
        {
            Track, IndividualNote
        }

        void Awake()
        {
            INSTANCE = this;
        }

        public void UpdateSyncSmoothing(bool smooth)
        {
            this.syncSmoothing = smooth;
        }

        public void Init()
        {
            songManager = SongManager.INSTANCE;
            tracks = GetComponentsInChildren<Track>();
            this.getMidiId();

            if (useNotePool)
                InitNotePool();
        }

        public void SetupForNewSong()
        {
            if (useNotePool)
                SetUpNotePool();
            else
                CreateAllNoteNow();
        }

        public void getMidiId()
        {
            GameObject songIndex = GameObject.Find("global");
            this.midiId = songIndex.GetComponent<GlobalControl>().getSongId();
        }

        public void UpdateTrack(float songPosition, float secPerBeat)
        {
            foreach (var track in tracks)
            {
                switch (syncMode)
                {
                    case SyncMode.Track:
                        var target = track.notesParent;
                        var songPositionInBeats = (songPosition + SongManager.INSTANCE.delay) / secPerBeat;
                        if (syncSmoothing)
                        {
                            var syncPosY = -songPositionInBeats * beatSize + track.lineArea.transform.localPosition.y + hitOffset;
                            target.Translate(new Vector3(0, -1, 0) * (1 / secPerBeat) * beatSize * Time.deltaTime);
                            //Smooth out the value with Time.deltaTime
                            // print(songPosition + " vs  " + syncPosY + " vs " + target.localPosition.y + " vs smooth " + (syncPosY + target.localPosition.y) / 2);
                            target.localPosition = new Vector3(0, (syncPosY + target.localPosition.y) / 2, 0);
                        }
                        else
                        {
                            target.localPosition = new Vector3(0, -songPositionInBeats * beatSize + track.lineArea.transform.localPosition.y + hitOffset, 0);
                        }
                        break;
                    case SyncMode.IndividualNote:
                        foreach (Note note in track.runtimeNote)
                        {
                            //This note object probably got destroyed
                            if (!note || !note.inUse) continue;
                            if (!note.gameObject.activeSelf)
                            {
                                note.gameObject.SetActive(true);
                            }

                            if (syncSmoothing)
                            {
                                //Offsetting the noteTime by 1 to prevent division by 0 error if the midi start at time = 0
                                var originalY = ((note.noteTime + 1) / secPerBeat) * beatSize;
                                note.transform.localPosition = Vector3.LerpUnclamped(new Vector3(0, originalY, 0), new Vector3(0, hitOffset, 0), (songPosition + 1) / (note.noteTime + 1));
                            }
                            else
                            {
                                var songPositionInBeats2 = (songPosition - note.noteTime) / secPerBeat;
                                var syncPosY = -songPositionInBeats2 * beatSize;
                                note.transform.localPosition = new Vector3(0, syncPosY + hitOffset, 0);
                            }
                        }
                        break;
                }
            }

            if (useNotePool)
                UpdateNoteInPool();
        }

        public void ClearAllTracks()
        {
            foreach (var track in tracks)
            {
                track.ResetTrack();

                if (useNotePool)
                    track.RecycleAllNotes(this);
                else
                    track.DestoryAllNotes();
            }
        }

        public void ResetNoteToPool(GameObject noteObject)
        {
            var note = noteObject.GetComponent<Note>();
            if (!note) return;
            note.ResetForPool();
            note.transform.SetParent(notesPoolParent);
            note.gameObject.SetActive(false);
            note.transform.localPosition = Vector3.zero;
        }

        private GameObject GetUnUsedNote(int noteType)
        {
            var note = pooledNotes.Find(x => !x.inUse && x.noteType == noteType);

            if (note == null)
            {
                note = GetNewNoteObject(noteType);
            }

            note.inUse = true;
            return note.gameObject;
        }

        private Note GetNewNoteObject(int noteType)
        {
            if (notePrefabMapping.notesPrefab[noteType].prefab == null)
            {
                Debug.LogError("The prefab type index at " + noteType + " shouldn't be null, please check the NotePrefabMapping asset");
            }
            var o = Instantiate(notePrefabMapping.notesPrefab[noteType].prefab);
            o.transform.SetParent(notesPoolParent);
            o.SetActive(false);

            var note = o.GetComponent<Note>();
            note.noteType = noteType;
            note.inUse = false;
            pooledNotes.Add(note);

            return note;
        }

        private void InitNotePool()
        {
            notesPoolParent = new GameObject("NotesPool").transform;
            notesPoolParent.SetParent(transform);
            for (int i = 0; i < notePrefabMapping.notesPrefab.Count; i++)
            {
                for (int j = 0; j < notePrefabMapping.notesPrefab[i].poolSize; j++)
                {
                    GetNewNoteObject(i);
                }
            }
        }

        private void SetUpNotePool()
        {
            for (int i = 0; i < tracks.Count(); i++)
            {
                var track = tracks[i];

                if (i > midiTracksMappingList[this.midiId].mapping.Count - 1)
                {
                    Debug.Log("Mapping has not enough track count!");
                    continue;
                }

                var x = midiTracksMappingList[this.midiId].mapping[i];

                track.allNotes = songManager.currnetNotes.Where(n =>
                {
                    return midiTracksMappingList[this.midiId].CompareMidiMapping(x, n);
                });

                //We clear previous notes object
                track.RecycleAllNotes(this);
            }
        }

        private void UpdateNoteInPool()
        {
            foreach (var track in tracks)
            {
                if (track.allNotes == null) continue;

                foreach (var note in track.allNotes)
                {
                    //We need to place this note ahead
                    if (!note.created && songManager.songPosition + poolLookAheadTime >= note.time)
                    {
                        note.created = true;

                        var noteType = notePrefabMapping.GetNoteType(note);
                        var newNoteObject = GetUnUsedNote(noteType);
                        track.AttachNote(newNoteObject);

                        InitNote(newNoteObject, note);
                    }
                }
            }
        }

        private void InitNote(GameObject newNoteObject, SongItem.MidiNote note)
        {
            var pos = Vector3.zero;
            var time = note.time;
            var beatUnit = time / songManager.secPerBeat;
            pos.y = beatUnit * beatSize + (songManager.delay / songManager.secPerBeat * beatSize);
            //The note is being positioned in the track and offset with the delay.

            newNoteObject.transform.localPosition = pos;

            var noteScript = newNoteObject.GetComponent<Note>();
            noteScript.InitNoteLength(note.noteLength);
            noteScript.noteTime = note.time;

            //For the SyncMode.IndividualNote, we activate the note object later on
            if (syncMode == SyncMode.Track)
                newNoteObject.SetActive(true);
        }

        private void CreateAllNoteNow()
        {
            for (int i = 0; i < tracks.Count(); i++)
            {
                var track = tracks[i];
                var x = midiTracksMappingList[this.midiId].mapping[i];

                track.allNotes = songManager.currnetNotes.Where(n =>
                {
                    return midiTracksMappingList[this.midiId].CompareMidiMapping(x, n);
                });

                //We clear previous notes object
                track.DestoryAllNotes();

                if (track.allNotes == null) continue;

                foreach (var note in track.allNotes)
                {
                    var newNoteObject = track.CreateNote(notePrefabMapping.GetNotePrefab(note));
                    InitNote(newNoteObject, note);
                }
            }
        }
    }
}