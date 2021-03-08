using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RhythmGameStarter
{
    public class PlaySong : MonoBehaviour
    {
        // Start is called before the first frame update
        //public int countDownTime;
        public Text countDownDisplay;

        IEnumerator CountdownToStart(int countDownTime)
        {
            countDownDisplay.gameObject.SetActive(true);
            while (countDownTime > 0)
            {
                countDownDisplay.text = countDownTime.ToString();
                yield return new WaitForSeconds(1f);
                countDownTime--;

            }
            countDownDisplay.text = "GO!";
            changeAudio();
            countDownDisplay.gameObject.SetActive(false);
        }

        void Start()
        {
            //countDownTime = 3;
            this.GetComponent<Button>().onClick.AddListener(playAudio);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void playAudio()
        {
            StartCoroutine(CountdownToStart(3));
        }

        public void changeAudio()
        {
            //AsyncOperation scene = SceneManager.LoadSceneAsync("StartMenu", LoadSceneMode.Additive);
            //scene.allowSceneActivation = false;
            GameObject play = GameObject.Find("Play");
            play.SetActive(false);

            GameObject bt = GameObject.Find("Button");
            bt.SetActive(false);

            GameObject songIndex = GameObject.Find("global");
            int idx = songIndex.GetComponent<GlobalControl>().getSongId();

            GameObject rhythmPlayer = GameObject.Find("RhythmGameStarter");

            rhythmPlayer.GetComponent<SongManager>().PlaySong(idx);
        }
    }
}

