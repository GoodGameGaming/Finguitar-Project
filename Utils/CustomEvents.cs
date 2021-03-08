using UnityEngine.Events;
using UnityEngine;

namespace RhythmGameStarter
{
    [System.Serializable]
    public class StringEvent : UnityEvent<string>
    {
    }

    [System.Serializable]
    public class BoolEvent : UnityEvent<bool>
    {
    }

    [System.Serializable]
    public class FloatEvent : UnityEvent<float>
    {
    }

    [System.Serializable]
    public class TouchEvent : UnityEvent<Touch>
    {
    }

    public class CollapsedEventAttribute : PropertyAttribute
    {
        public bool visible;

        public CollapsedEventAttribute()
        {

        }
    }
}
