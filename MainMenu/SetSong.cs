using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace RhythmGameStarter
{
    public class SetSong : MonoBehaviour
    {
        void Start()
        {
            this.GetComponent<Button>().onClick.AddListener(changeAudio);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void changeAudio()
        {
            int idx = int.Parse(this.name);
            GameObject songIndex = GameObject.Find("global");
            if(songIndex.GetComponent<GlobalControl>().checkLevel(idx-1)){
                songIndex.GetComponent<GlobalControl>().setSongId(idx-1);
            }
            else{
                GameObject lockMessage = GameObject.Find("LockMessage");
                GameObject obj = lockMessage.transform.Find("Message").gameObject;
                obj.SetActive(true);
                songIndex.GetComponent<GlobalControl>().setSongId(idx - 1);
            }
        }
    }
}

