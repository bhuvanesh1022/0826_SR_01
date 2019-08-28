using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statistics : MonoBehaviour
{
    // Start is called before the first frame update
    public static Statistics stats;

    public List<int> StartTrackSelect = new List<int>();
    public List<float> FinishTrackSelect = new List<float>();

    public int TotalWin;
    public int TotalLose;
    public int Stunned = 0;
    public int StunHit = 0;
    public int StunMissed = 0;
    public float TimeSpentInMenu = 0;
    public float TimeSpentInMenusave = 0;


    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
       //   PlayerPrefs.DeleteAll();
      
        StartCoroutine(SaveValueUpdate());
        stats = this;
    }

    IEnumerator SaveValueUpdate()
    {
        while(true)
        {
            yield return new WaitForSeconds(5f);

        }
    }
    public void PrefFloat(string name)
    {
        if (PlayerPrefs.GetFloat(name) == 0.0f)
        {
            PlayerPrefs.SetFloat(name, 1);

        }
        else
        {
            float num1 = PlayerPrefs.GetFloat(name);
            num1 = num1 + 1;
            PlayerPrefs.SetFloat(name, num1);
        }
        Refresh();
    }
    public void Pref(string name)
    {
        if (PlayerPrefs.GetInt(name) == 0.0f)
        {
            PlayerPrefs.SetInt(name, 1);

        }
        else
        {
           int num1 = PlayerPrefs.GetInt(name);
            num1 = num1 + 1;
            PlayerPrefs.SetInt(name, num1);
        }
        Refresh();
    }

    public void Refresh()
    {
        TotalWin = PlayerPrefs.GetInt("TotalWin");
        TotalLose = PlayerPrefs.GetInt("TotalLose");

        for(int i=0;i<2;i++)
        {
            StartTrackSelect[i] = PlayerPrefs.GetInt("StartTrack" + i);
            FinishTrackSelect[i]= PlayerPrefs.GetInt("FinishTrack" + i);
            FinishTrackSelect[i] = FinishTrackSelect[i] / 2;
        }
        Stunned = PlayerPrefs.GetInt("Stunned");
        StunHit = PlayerPrefs.GetInt("StunHit");
        StunMissed = PlayerPrefs.GetInt("StunMissed");
        TimeSpentInMenusave = PlayerPrefs.GetFloat("TimeSpentInMenu");


    }

    public void finishTrackSelect(int temp)
    {
        if (PlayerPrefs.GetFloat("FinishTrack" + temp) == 0.0f)
        {
            PlayerPrefs.SetFloat("FinishTrack" + temp, 0.5f);

        }
        else
        {
            FinishTrackSelect[temp] = PlayerPrefs.GetFloat("FinishTrack" + temp);
            FinishTrackSelect[temp] = FinishTrackSelect[temp] + 0.5f;
            PlayerPrefs.SetFloat("FinishTrack" + temp, FinishTrackSelect[temp]);
        }
    }

    // Update is called once per frame
    void Update()
    {

        if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name != "0716Level01")
        {
            if(TimeSpentInMenu>60f)
            {

                PrefFloat("TimeSpentInMenu");
                TimeSpentInMenu = 0;
            }
            TimeSpentInMenu += Time.deltaTime;
        }
    }
}
