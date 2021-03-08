using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RhythmGameStarter
{
    public class TouchInputHandler : MonoBehaviour
    {
        void Update()
        {
            //Input
            for (int i = 0; i < Input.touchCount; ++i)
            {
                var touch = Input.GetTouch(i);

                //We also see if this touch was used by the UI
                if (touch.phase == TouchPhase.Began && !EventSystem.current.IsPointerOverGameObject(touch.fingerId))
                    TouchAt(touch);
            }
        }

        private void TouchAt(Touch touch)
        {
            Ray ray = Camera.main.ScreenPointToRay(touch.position);
            // print(touch.position);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var t = hit.rigidbody.GetComponent<TrackTriggerArea>();
                if (!t) return;

                t.TriggerNote(touch);
            }
        }
    }
}