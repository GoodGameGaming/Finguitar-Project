using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace RhythmGameStarter
{
    public class GlobalControl : MonoBehaviour
    {

        public static GlobalControl Instance;


        //要保存使用的数据;
        public int EXP = 0;
        public int level = 1;
        public string playerName;
        public int nextSong;
        public int[] topScores;
        bool unlock;
        int hasUnlock;

        [Header("[Required Level]")]
        public List<int> levelRequired;
        public bool canPlay = false;
        //初始化
        private void Awake()
        {
            if (Instance == null)
            {
                DontDestroyOnLoad(gameObject);
                Instance = this;
            }
            else if (Instance != null)
            {
                Destroy(gameObject);
            }
            topScores = new int[3];
            unlock = false;
            
            //读取之前数据
            if (PlayerPrefs.HasKey("EXP"))
            {
                //PlayerPrefs.SetInt("EXP", 0);
                this.EXP = PlayerPrefs.GetInt("EXP");
            }

            if (PlayerPrefs.HasKey("hasUnlocked"))
            {
                //PlayerPrefs.SetInt("hasUnlocked", 0);
                this.hasUnlock = PlayerPrefs.GetInt("hasUnlocked");
            }

            if (PlayerPrefs.HasKey("level"))
            {
               //PlayerPrefs.SetInt("level", 1);
                this.level = PlayerPrefs.GetInt("level");
            }

            if (PlayerPrefs.HasKey("cloudTopScore"))
            {
                //PlayerPrefs.SetInt("cloudTopScore", 0);
                topScores[0] = PlayerPrefs.GetInt("cloudTopScore");
                
            }

            if (PlayerPrefs.HasKey("flowerTopScore"))
            {
                //PlayerPrefs.SetInt("flowerTopScore", 0);
                topScores[1] = PlayerPrefs.GetInt("flowerTopScore");
            }

            if (PlayerPrefs.HasKey("timeTopScore"))
            {
                //PlayerPrefs.SetInt("timeTopScore", 0);
                topScores[2] = PlayerPrefs.GetInt("timeTopScore");
            }

        }

        public void setUnlock(bool t)
        {
            unlock = t;
        }

        public bool getUnlock()
        {
            return unlock;
        }
        public void SetPlayerName(string name)
        {
            this.playerName = name;
        }

        public void SetNextSong(int songId)
        {
            this.nextSong = songId;
        }

        public void LevelUp(int expGained)
        {
            
            if (expGained > 1000)
            {
                this.EXP = this.EXP + expGained;
            }

            int levelUpRequired = this.level * 1000;
            while(this.EXP > levelUpRequired)
            {
                this.level = this.level + 1;
                this.EXP = this.EXP- levelUpRequired;
                levelUpRequired = this.level * 1000;
            }
 
            PlayerPrefs.SetInt("level", this.level);
            PlayerPrefs.SetInt("EXP", this.EXP);
            if(nextSong == 0 && expGained > topScores[0])
            {
                topScores[nextSong] = expGained;
                PlayerPrefs.SetInt("cloudTopScore", expGained);
            }else if(nextSong == 1 && expGained > topScores[1])
            {
                topScores[nextSong] = expGained;
                PlayerPrefs.SetInt("flowerTopScore", expGained);
            }else if(nextSong == 2 && expGained > topScores[2])
            {
                topScores[nextSong] = expGained;
                PlayerPrefs.SetInt("timeTopScore", expGained);
            }
        }

        public int getSongId()
        {
            return this.nextSong;
        }

        public int getLevel()
        {
            return this.level;
        }

        public int getEXP()
        {
            return this.EXP;
        }

        public int[] getTopScore()
        {
            int[] a = new int[3];
            a[0] = this.topScores[0];
            a[1] = this.topScores[1];
            a[2] = this.topScores[2];
            return a;
        }
        public List<int> getRequiredLevel()
        {
            return this.levelRequired;
        }
      
        public bool getCanPlay()
        {
            return canPlay;
        }

        public bool checkLevel(int idx)
        {
            if(this.level<this.levelRequired[idx]){
                return false;
            }
            else{
                return true;
            }
        }

        public void setSongId(int idx)
        {
            this.nextSong = idx;
        }

        public void setHasUnlock(int t)
        {
            hasUnlock = t;
            PlayerPrefs.SetInt("hasUnlocked", this.hasUnlock);

        }

        public int getHasUnlock()
        {
            return hasUnlock;
        }
    }
}



