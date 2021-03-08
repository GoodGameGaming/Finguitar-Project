using UnityEngine;

namespace RhythmGameStarter
{
    public class ComboSystem : MonoBehaviour
    {
        public static ComboSystem INSTANCE;

        private bool isShowing;
        [Header("[Events]")]
        [CollapsedEvent]
        public StringEvent onComboUpdate;
        [CollapsedEvent]
        public BoolEvent onVisibilityChange;
        [CollapsedEvent]
        public BoolEvent onMissChange;
        [CollapsedEvent]
        public BoolEvent onOkChange;
        [CollapsedEvent]
        public BoolEvent onPerfectChange;

        GameObject combo;

        void Awake()
        {
            INSTANCE = this;
           
        }

        void Start()
        {
            UpdateComboDisplay();
            combo = GameObject.Find("Combo");
            combo.SetActive(false);
        }
        void Update()
        {

        }

        public void AddCombo(int addCombo, float deltaDiff, int score)
        {
            combo.SetActive(true);
            //levelIdx: -1 miss, 0 perfect, 1 ok
            int levelIdx = StatsSystem.INSTANCE.AddCombo(addCombo, deltaDiff, score);
            
            onMissChange.Invoke(false);
            onOkChange.Invoke(false);
            onPerfectChange.Invoke(false);
            
            if (levelIdx == 0)
            {
                onPerfectChange.Invoke(true);
            }
            else if(levelIdx == 1)
            {
                onOkChange.Invoke(true);
            }
            if (!isShowing)
            {
                isShowing = true;
                onVisibilityChange.Invoke(isShowing);
            }
            
            UpdateComboDisplay();
        }

        public void BreakCombo()
        {
            StatsSystem.INSTANCE.AddMissed(1);
            StatsSystem.INSTANCE.combo = 0;

            isShowing = false;
            onVisibilityChange.Invoke(isShowing);
            onPerfectChange.Invoke(false);
            onOkChange.Invoke(false);
            onMissChange.Invoke(true);
            //print(miss++);
        }

        public void UpdateComboDisplay()
        {
            onComboUpdate.Invoke(StatsSystem.INSTANCE.combo.ToString());
            //onMissChange.Invoke(false);
        }
    }
}