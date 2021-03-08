using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using UnityEngine.UI;
using TMPro;
//using UnityEngine.Image;
using System.Collections;

namespace RhythmGameStarter
{
    public class SongManager : MonoBehaviour
    {
        public static SongManager INSTANCE;
        public int countDownTime;
        public Text countDownDisplay;
        public Image mask;
        public Image bar;
        GameObject levelMessage;
        GameObject scoreMessage;

        GameObject lvlObj;
        GameObject scoreObj;
        GameObject m2;
        GameObject effect;
        ParticleSystem e;
        IEnumerator CountdownToResume()
        {
            countDownDisplay.gameObject.SetActive(true);
            while (countDownTime > 0)
            {
                countDownDisplay.text = countDownTime.ToString();
                yield return new WaitForSeconds(1f);
                countDownTime--;

            }
            countDownDisplay.text = "GO!";
            resumeBack();
            countDownDisplay.gameObject.SetActive(false);
        }
        void Awake()
        {
            INSTANCE = this;
        }

        public AudioSource audioSource;

        [Header("[Properties]")]
        [Space]
        public bool playOnAwake = true;
        public SongItem defaultSong;
        public float delay;

        [Header("[Songlist]")]
        public List<SongItem> songList;


        [Header("[Display]")]
        public bool progressAsPercentage = true;

        [HideInInspector]
        public float secPerBeat;
        [HideInInspector]
        public float songPosition;
        [HideInInspector]
        public IEnumerable<SongItem.MidiNote> currnetNotes;

        [Header("[Events]")]
        [CollapsedEvent]
        public FloatEvent onSongProgress;
        [CollapsedEvent]
        public StringEvent onSongProgressDisplay;
        [CollapsedEvent]
        public UnityEvent onSongStart;
        [CollapsedEvent]
        public UnityEvent onSongFinished;

        private bool songHasStarted;
        private bool songStartEventInvoked;

        [HideInInspector]
        public bool songPaused;

        private double dspStartTime;
        private double dspPausedTime;
        private double accumulatedPauseTime;

        public int finalScore = 0;
        int previousScore = 0;
        int previousLevel = 1;
        int score = 0;
        int id = 0;
        List<int> requiredLv;
        void Start()
        {
            TrackManager.INSTANCE.Init();
            bar = GameObject.FindWithTag("RadioProgressBar").GetComponent<Image>();
            levelMessage = GameObject.Find("LevelUpMessage");
            scoreMessage = GameObject.Find("NewScoreMessage");
            //effect = GameObject.Find("effect5");
            //e = effect.GetComponent<ParticleSystem>();
            lvlObj = levelMessage.transform.Find("Message").gameObject;
            scoreObj = scoreMessage.transform.Find("Message").gameObject;

            bar.enabled = false;
            //levelMessage.SetActive(false);
            //scoreMessage.SetActive(false);
            scoreObj.SetActive(false);
            lvlObj.SetActive(false);
            //if (e.isPlaying) e.Stop();
            if (playOnAwake && defaultSong)
            {
                PlaySong(0);
            }
        }

        public void PlaySong(int songId)
        {
            
            bar.enabled = true;
            songPaused = false;
            songHasStarted = true;
            accumulatedPauseTime = 0;
            dspPausedTime = 0;
            songPosition = -1;

            SongItem songItem = this.songList[songId];

            audioSource.clip = songItem.clip;

            songItem.ResetNotesState();
            currnetNotes = songItem.notes;
            secPerBeat = 60f / songItem.bpm;

            //Starting the audio play back
            dspStartTime = AudioSettings.dspTime;
            audioSource.PlayScheduled(AudioSettings.dspTime + delay);

            TrackManager.INSTANCE.SetupForNewSong();
        }

        public void PauseSong()
        {
            if (songPaused) return;

            songPaused = true;
            audioSource.Pause();

            dspPausedTime = AudioSettings.dspTime;
        }

        public void ResumeSong()
        {
            if (!songPaused) return;

            StartCoroutine(CountdownToResume());
        }

  

        public void resumeBack()
        {
            songPaused = false;
            audioSource.Play();

            accumulatedPauseTime += AudioSettings.dspTime - dspPausedTime;
        }

        public void StopSong(bool dontInvokeEvent = false)
        {
            audioSource.Stop();
            songHasStarted = false;
            songStartEventInvoked = false;

            mask = GameObject.FindWithTag("Fill").GetComponent<Image>();
            mask.fillAmount = 0.0f;

            bar = GameObject.FindWithTag("RadioProgressBar").GetComponent<Image>();
            bar.enabled = false;

            StatsSystem.INSTANCE.reset();

            if (!dontInvokeEvent)
                onSongFinished.Invoke();

            TrackManager.INSTANCE.ClearAllTracks();
        }

        public void showNewScore()
        {
            if (score > previousScore)
            {

                //scoreMessage.SetActive(true);
                scoreObj.SetActive(true);
                m2.GetComponent<TMP_Text>().SetText("New Record!\n Score: " + score + "!");
                //e.Play();


            }
        }

        void Update()
        {
            if (!songStartEventInvoked && songHasStarted && songPosition >= 0)
            {
                songStartEventInvoked = true;
                onSongStart.Invoke();
            }

            //Sync the tracks position with the audio
            if (!songPaused && songHasStarted)
            {
                songPosition = (float)(AudioSettings.dspTime - dspStartTime - delay - accumulatedPauseTime);

                TrackManager.INSTANCE.UpdateTrack(songPosition, secPerBeat);

                onSongProgress.Invoke(songPosition);
                if (songPosition >= 0)
                {
                    if (progressAsPercentage)
                    {
                        onSongProgressDisplay.Invoke(Math.Truncate(songPosition / audioSource.clip.length * 100) + "%");
                        float fill = (float)songPosition / (float)audioSource.clip.length;
                        mask = GameObject.FindWithTag("Fill").GetComponent<Image>();
                        mask.fillAmount = fill;
                    }

                    else
                    {
                        var now = new DateTime((long)songPosition * TimeSpan.TicksPerSecond);
                        onSongProgressDisplay.Invoke(now.ToString("mm:ss"));
                    }
                }
            }

            GameObject glo = GameObject.Find("global");

            if (songHasStarted && audioSource.clip && songPosition >= audioSource.clip.length)
            {
                songHasStarted = false;
                songStartEventInvoked = false;
                onSongFinished.Invoke();
                score = int.Parse(GameObject.FindWithTag("Score").GetComponent<TMP_Text>().text);
                if (finalScore == 0)
                {
                    this.finalScore = score;
                }
                //Debug.Log(finalScore);
                //Debug.Log(GameObject.FindWithTag("EXP").GetComponent<Image>().fillAmount);
                
                id = glo.GetComponent<GlobalControl>().getSongId();
                previousScore = glo.GetComponent<GlobalControl>().getTopScore()[id];
                previousLevel = glo.GetComponent<GlobalControl>().getLevel();
                glo.GetComponent<GlobalControl>().LevelUp(score);
                float preFill = glo.GetComponent<GlobalControl>().getEXP();
                Image EXP_BAR = GameObject.FindWithTag("EXP").GetComponent<Image>();
                EXP_BAR.fillAmount = preFill;
                
                if (EXP_BAR.fillAmount == 1)
                {
                    EXP_BAR.fillAmount = 0;
                }
                EXP_BAR.fillAmount = (float)(EXP_BAR.fillAmount + 0.01);
                finalScore = finalScore - 10;

                int level = glo.GetComponent<GlobalControl>().getLevel();
                requiredLv = glo.GetComponent<GlobalControl>().getRequiredLevel();
                if (level >= requiredLv[2])
                {
                    glo.GetComponent<GlobalControl>().setUnlock(true);
                }
                GameObject m = lvlObj.transform.Find("message").gameObject;
                m2 = scoreObj.transform.Find("message").gameObject;
               
                if (level > previousLevel)
                {
                    //levelMessage.SetActive(true);
                    lvlObj.SetActive(true);
                    //e.Play();
                    m.GetComponent<TMP_Text>().SetText("Level Up!\n New Level is " + level + "!");

                }
                else
                {
                    showNewScore();
                }

            }

            if (finalScore > 0)
            {
                //Debug.Log(finalScore);
                //Debug.Log(GameObject.FindWithTag("EXP").GetComponent<Image>().fillAmount);
                Image EXP_BAR = GameObject.FindWithTag("EXP").GetComponent<Image>();
                if (EXP_BAR.fillAmount == 1)
                {
                    EXP_BAR.fillAmount = 0;
                }
                EXP_BAR.fillAmount = (float)(EXP_BAR.fillAmount + 0.01);
                finalScore = finalScore - 10;

               
               
            }
        }

            
            
    }
}