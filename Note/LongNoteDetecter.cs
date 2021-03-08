using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class LongNoteDetecter : MonoBehaviour
    {
        private SpriteRenderer spriteRenderer;

        [HideInInspector]
        public bool exitedLineArea = false;

        void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void OnTouchDown()
        {
            var c = spriteRenderer.color;
            c.a = 0.5f;
            spriteRenderer.color = c;
        }

        public void OnTouchUp()
        {
            var c = spriteRenderer.color;
            c.a = 1f;
            spriteRenderer.color = c;
        }

        void OnTriggerExit(Collider col)
        {
            if (col.tag == "LineArea")
            {
                exitedLineArea = true;
            }
        }
    }
}