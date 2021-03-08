using System;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class SongItem : ScriptableObject
    {
        public AudioClip clip;
        public int bpm;
        public List<MidiNote> notes = new List<MidiNote>();

        [Serializable]
        public class MidiNote
        {
            public NoteName noteName;
            public int noteOctave;
            public float time;
            public float noteLength;

            [NonSerialized]
            public bool created;
        }

        public void ResetNotesState()
        {
            notes.ForEach(x => x.created = false);
        }

        public enum NoteName
        {
            C = 0,

            CSharp = 1,

            D = 2,

            DSharp = 3,

            E = 4,

            F = 5,
            FSharp = 6,

            G = 7,

            GSharp = 8,

            A = 9,

            ASharp = 10,

            B = 11
        }
    }
}