using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace RhythmGameStarter
{
    public class PlayButton : MonoBehaviour
    {
        bool canPlay;
        int id;
        // Start is called before the first frame update
        void Start()
        {
            
            GetComponent<Button>().onClick.AddListener(StartGame);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void StartGame()
        {
            GameObject glo = GameObject.Find("global");
            canPlay = glo.GetComponent<GlobalControl>().getCanPlay();
            id = glo.GetComponent<GlobalControl>().getSongId();
            if (id==2 && !canPlay)
            {
                GameObject dia = GameObject.FindWithTag("Dialog");
                dia.SetActive(true);
               
            }else{
                SceneManager.LoadScene("Horizontal6TracksDemo");
            }
           

        }
    }
}
