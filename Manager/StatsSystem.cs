using System;
using System.Collections.Generic;
using UnityEngine;

namespace RhythmGameStarter
{
    public class StatsSystem : MonoBehaviour
    {
        public static StatsSystem INSTANCE;

        void Awake()
        {
            INSTANCE = this;
        }


        [HideInInspector]
        public int combo;

        [HideInInspector]
        public int maxCombo;

        [HideInInspector]
        public int missed;

        [HideInInspector]
        public float score;


        public List<HitLevel> levels;

        [Header("[Events]")]
        [CollapsedEvent]
        public StringEvent onComboStatusUpdate;
        [CollapsedEvent]
        public StringEvent onScoreUpdate;
        [CollapsedEvent]
        public StringEvent onMaxComboUpdate;
        [CollapsedEvent]
        public StringEvent onMissedUpdate;

        [CollapsedEvent]
        public StringEvent onGradeUpdate;


        [Serializable]
        public class HitLevel
        {
            public string name;
            public float threshold;
            [HideInInspector]
            public int count;
            public float scorePrecentage = 1;
            public StringEvent onCountUpdate;
        }

        GameObject glo;
        int totalScore;

        public void AddMissed(int addMissed)
        {
            missed += addMissed;
            onMissedUpdate.Invoke(missed.ToString());
        }

        void Start()
        {
            //UpdateScoreDisplay();
        }

        public int AddCombo(int addCombo, float deltaDiff, int addScore)
        {
            // print(deltaDiff);
            combo += addCombo;
            if (combo > maxCombo)
            {
                maxCombo = combo;
                onMaxComboUpdate.Invoke(maxCombo.ToString());
            }

            for (int i = 0; i < levels.Count; i++)
            {
                var x = levels[i];
                if (deltaDiff <= x.threshold)
                {
                    x.count++;
                    score += ((float)addScore * x.scorePrecentage);
                    x.onCountUpdate.Invoke(x.count.ToString());
                    UpdateScoreDisplay();
                    onComboStatusUpdate.Invoke(x.name);
                    //print(x.name);
                    return i;
                }
            }

            //When no level matched
            onComboStatusUpdate.Invoke("");
            return -1;

        }

        public void reset()
        {
            combo = 0;
            maxCombo = 0;
            score = 0;
            missed = 0;

            for (int i = 0; i < levels.Count; i++)
            {
                var x = levels[i];
                x.count = 0;
            }
            UpdateScoreDisplay();

        }

        public void UpdateScoreDisplay()
        {
            onScoreUpdate.Invoke(score.ToString());
            glo = GameObject.Find("global");
            int songId = glo.GetComponent<GlobalControl>().getSongId();

            if (songId == 0)
            {
                totalScore = 54 * 100;
            }
            else if (songId == 1)
            {
                totalScore = 220 * 100;
            }
            else if (songId == 2)
            {
                totalScore = 341 * 100;
            }


            if (score >= totalScore * 0.25)
            {
                onGradeUpdate.Invoke("C");
            }
            else
            {
                onGradeUpdate.Invoke("F");
            }
            if (score >= totalScore * 0.5)
            {
                onGradeUpdate.Invoke("B");
            }
            if (score >= totalScore * 0.65)
            {
                onGradeUpdate.Invoke("A");
            }
            if (score >= totalScore * 0.8)
            {
                onGradeUpdate.Invoke("S");
            }
            //print(score);
        }
    }
}