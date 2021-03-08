using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class KeyboardInputHandler : MonoBehaviour
    {
        public KeyCode up;
        public KeyCode down;
        public KeyCode left;
        public KeyCode right;
        public List<string> keyMapping;

        private NoteArea[] noteAreas;

        void Start()
        {
            noteAreas = GetComponentsInChildren<NoteArea>();
            for (int i = 0; i < noteAreas.Length; i++)
            {
                var noteArea = noteAreas[i];
                noteArea.keyboardInputHandler = this;

                var key = keyMapping[i];
                noteArea.key = key;
            }
        }
    }
}