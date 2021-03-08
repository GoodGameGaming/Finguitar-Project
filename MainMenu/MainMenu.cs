using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
namespace RhythmGameStarter
{
    public class MainMenu : MonoBehaviour
    {
        bool canPlay;
        int id;
        GameObject dia;
        GameObject glo;
        GameObject unlockMessage;
        // Start is called before the first frame update
        void Start()
        {
            //dia = GameObject.FindWithTag("Dialog");
            //dia.SetActive(false);
            glo  = GameObject.Find("global");
            int level = glo.GetComponent<GlobalControl>().getLevel();
            int EXP = glo.GetComponent<GlobalControl>().getEXP();
            List<int> requiredLv = glo.GetComponent<GlobalControl>().getRequiredLevel();
            GameObject player = GameObject.FindWithTag("Player");
            canPlay = glo.GetComponent<GlobalControl>().getCanPlay();
            Component[] childs = player.GetComponentsInChildren(typeof(TMP_Text), true);
            unlockMessage = GameObject.Find("unlockMessage");
            GameObject um = unlockMessage.transform.Find("Message").gameObject;
            bool unlock = glo.GetComponent<GlobalControl>().getUnlock();
            int hasUnlock = glo.GetComponent<GlobalControl>().getHasUnlock();

            if (unlock && hasUnlock==0)
            {
                um.SetActive(true);
                glo.GetComponent<GlobalControl>().setHasUnlock(1);
            }
            foreach (TMP_Text x in childs)
            {
                //print(x.name);
                if (x.name.Equals("Level"))
                {
                    x.GetComponent<TMP_Text>().SetText(level.ToString());
                }
                if (x.name.Equals("EXP"))
                {
                    x.GetComponent<TMP_Text>().SetText(EXP.ToString());
                }
            }

            GameObject fill = GameObject.Find("Fill");
            fill.GetComponent<Image>().fillAmount = (float)EXP/1000;

            GameObject songs = GameObject.FindWithTag("Songs");
            int[] topScores = glo.GetComponent<GlobalControl>().getTopScore();
            for(int i =0;i<5;i++){
                GameObject item = songs.transform.GetChild(i).gameObject;
                Component[] records = item.GetComponentsInChildren(typeof(TMP_Text), true);
                foreach (TMP_Text x in records)
                {
                    //print(item.name+":"+x.name);
                    if (x.name.Equals("TopScore") && i+1<=topScores.Length)
                    {
                        x.GetComponent<TMP_Text>().SetText(topScores[i].ToString());
                    }
                    else if (x.name.Equals("LevelRequired"))
                    {
                        x.GetComponent<TMP_Text>().SetText(requiredLv[i].ToString());
                    }
                    if(level>=requiredLv[i]){
                        item.transform.GetChild(4).gameObject.SetActive(false);
                    }

                }
            }
            
            GameObject play = GameObject.Find("button_play");
            play.GetComponent<Button>().onClick.AddListener(StartGame);
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void StartGame()
        {
            glo = GameObject.Find("global");
            canPlay = glo.GetComponent<GlobalControl>().getCanPlay();
            id = glo.GetComponent<GlobalControl>().getSongId();
            List<int> requiredLv = glo.GetComponent<GlobalControl>().getRequiredLevel();
            int level = glo.GetComponent<GlobalControl>().getLevel();
            dia = GameObject.Find("LockMessage");
            GameObject obj = dia.transform.Find("Message").gameObject;
            if (level < requiredLv[id])
            {

                obj.SetActive(true);

            }
            else
            {
                SceneManager.LoadScene("Horizontal6TracksDemo");
            }


        }
    }
}
