using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ControlData : MonoBehaviourPunCallbacks
{

    public float BaseSpeed ;
    public float TargetSpeed ;
    public float MaxRunForce ;
     public float m_JumpForce;                          // Amount of force added when the player jumps.
    public float playerGravityScale;
    public float terminalVelocity;
    public float walljumpAmplitudeLeft;
    public float walljumpForceLeft;
    public float WallSlideGravity;

    public AudioControl Ac;
    public Text BaseSpeedtxt;
    public Text TargetSpeedTxt;
    public Text MaxRunForceTxt;
    public Text m_JumpForceTxt;
    public Text playerGravityScaleTxt;
    public Text terminalVelocityTxt;
    public Text walljumpAmplitudeLeftTxt;
    public Text walljumpForceLeftTxt;
    public Text WallSlideGravityTxt;




    public GameObject CompleteMenu;

    public Text BaseSpeedtxt1;
    public Text TargetSpeedTxt1;
    public Text MaxRunForceTxt1;
    public Text m_JumpForceTxt1;
    public Text playerGravityScaleTxt1;
    public Text terminalVelocityTxt1;
    public Text walljumpAmplitudeLeftTxt1;
    public Text walljumpForceLeftTxt1;
    public Text WallSlideGravityTxt1;


    // Start is called before the first frame update
    public void deleteall() {
        PlayerPrefs.DeleteAll();
        ValueReset();
    }

    public void ValueReset() {
        if (PlayerPrefs.GetFloat("BaseSpeed") == 0.0f) {
            PlayerPrefs.SetFloat("BaseSpeed", 30f);
            BaseSpeedtxt.text = "30";
            BaseSpeed = 30;
        }
        else {
            BaseSpeed = PlayerPrefs.GetFloat("BaseSpeed");
            BaseSpeedtxt.text = BaseSpeed.ToString();

        }


        if (PlayerPrefs.GetFloat("TargetSpeed") == 0.0f) {
            PlayerPrefs.SetFloat("TargetSpeed", 60f);
            TargetSpeedTxt.text = "60";
            TargetSpeed = 60;

        }
        else {
            TargetSpeed = PlayerPrefs.GetFloat("TargetSpeed");
            TargetSpeedTxt.text = TargetSpeed.ToString();

        }

        if (PlayerPrefs.GetFloat("MaxRunForce") == 0.0f) {
            PlayerPrefs.SetFloat("MaxRunForce", 100f);
            MaxRunForceTxt.text = "100";
            MaxRunForce = 100;
        }
        else {
            MaxRunForce = PlayerPrefs.GetFloat("MaxRunForce");
            MaxRunForceTxt.text = MaxRunForce.ToString();
        }


        if (PlayerPrefs.GetFloat("m_JumpForce") == 0.0f) {
            PlayerPrefs.SetFloat("m_JumpForce", 30f);
            m_JumpForce = 30;
            m_JumpForceTxt.text = m_JumpForce.ToString();

        }
        else {
            m_JumpForce = PlayerPrefs.GetFloat("m_JumpForce");
            m_JumpForceTxt.text = m_JumpForce.ToString();

        }


        if (PlayerPrefs.GetFloat("playerGravityScale") == 0.0f) {
            PlayerPrefs.SetFloat("playerGravityScale", 7.5f);

            //  GetComponent<Rigidbody2D>().gravityScale = 20f;
            playerGravityScale = 7.5f;
            playerGravityScaleTxt.text = "7.5";


        }
        else {
            playerGravityScale = PlayerPrefs.GetFloat("playerGravityScale");
            playerGravityScaleTxt.text = playerGravityScale.ToString();
            //  GetComponent<Rigidbody2D>().gravityScale = GetComponent<cc>().playerGravityScale;
            //controldata.playerGravityScale = GetComponent<cc>().playerGravityScale;

        }


        if (PlayerPrefs.GetFloat("terminalVelocity") == 0.0f) {
            PlayerPrefs.SetFloat("terminalVelocity", -100f);
            //  GetComponent<cc>().terminalVelocityTxt.text = "-100";
            terminalVelocity = -100;

        }
        else {
            terminalVelocity = PlayerPrefs.GetFloat("terminalVelocity");
            terminalVelocityTxt.text = terminalVelocity.ToString();
            //    terminalVelocity = GetComponent<cc>().terminalVelocity;

        }

        if (PlayerPrefs.GetFloat("walljumpAmplitudeLeft") == 0.0f) {
            PlayerPrefs.SetFloat("walljumpAmplitudeLeft", 3);
            walljumpAmplitudeLeftTxt.text = "3";
            walljumpAmplitudeLeft = 3;

        }
        else {
            walljumpAmplitudeLeft = PlayerPrefs.GetFloat("walljumpAmplitudeLeft");
            walljumpAmplitudeLeftTxt.text = walljumpAmplitudeLeft.ToString();
        }

        if (PlayerPrefs.GetFloat("walljumpForceLeft") == 0.0f) {
            PlayerPrefs.SetFloat("walljumpForceLeft", 12.5f);
            walljumpForceLeftTxt.text = "12.5";
            walljumpForceLeft = 12.5f;


        }
        else {
            walljumpForceLeft = PlayerPrefs.GetFloat("walljumpForceLeft");
            walljumpForceLeftTxt.text = walljumpForceLeft.ToString();
        }
        if (PlayerPrefs.GetFloat("WallSlideGravity") == 0.0f) {
            PlayerPrefs.SetFloat("WallSlideGravity", 12f);
            WallSlideGravityTxt.text = "12";
            WallSlideGravity = 12f;
        }
        else {
            WallSlideGravity = PlayerPrefs.GetFloat("WallSlideGravity");
            WallSlideGravityTxt.text = WallSlideGravity.ToString();
        }
    }
    void Start()
    {
     //  PlayerPrefs.DeleteAll();
        PhotonNetwork.ConnectUsingSettings();

        DontDestroyOnLoad(this.gameObject);
        ValueReset();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseBtn()
    {
        Application.Quit();
    }

    public void ReloadBtn()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Multiplayer()
    {
         UnityEngine.SceneManagement.SceneManager.LoadScene("mainMenu");
       // CompleteMenu.gameObject.SetActive(true);

     BaseSpeedtxt1.text=BaseSpeed.ToString();
     TargetSpeedTxt1.text=TargetSpeed.ToString();
     MaxRunForceTxt1.text=MaxRunForce.ToString();
     m_JumpForceTxt1.text=m_JumpForce.ToString();
     playerGravityScaleTxt1.text=playerGravityScale.ToString();
     terminalVelocityTxt1.text=terminalVelocity.ToString();
     walljumpAmplitudeLeftTxt1.text=walljumpAmplitudeLeft.ToString();
     walljumpForceLeftTxt1.text=walljumpForceLeft.ToString();
     WallSlideGravityTxt1.text=WallSlideGravity.ToString();
     
     }

    public void BaseSpeedFunc(string num)
    {
        BaseSpeed = float.Parse(num);
        PlayerPrefs.SetFloat("BaseSpeed", BaseSpeed);

    }
    public void TargetSpeedFunc(string num)
    {
        TargetSpeed = float.Parse(num);
        PlayerPrefs.SetFloat("TargetSpeed", TargetSpeed);

    }
    public void MaxRunForceFunc(string num)
    {
        MaxRunForce = float.Parse(num);
        PlayerPrefs.SetFloat("MaxRunForce", MaxRunForce);

    }
    public void JumpSpeed(string num)
    {
       m_JumpForce = float.Parse(num);
            PlayerPrefs.SetFloat("m_JumpForce", m_JumpForce);

    }
    public void TerminalFallSpeed(string num)
    {
       terminalVelocity = float.Parse(num);
        PlayerPrefs.SetFloat("terminalVelocity", terminalVelocity);

    }
    public void playerGravityScaleSpeed(string num)
    {
        playerGravityScale = float.Parse(num);
        PlayerPrefs.SetFloat("playerGravityScale", playerGravityScale);

    }
    public void walljumpAmplitudeSpeed(string num)
    {
        walljumpAmplitudeLeft = float.Parse(num);
        PlayerPrefs.SetFloat("walljumpAmplitudeLeft", walljumpAmplitudeLeft);

    }
    public void walljumpForceSpeed(string num)
    {
        walljumpForceLeft = float.Parse(num);
        PlayerPrefs.SetFloat("walljumpForceLeft", walljumpForceLeft);

    }
    public void WallSlideGravitySpeed(string num)
    {
        WallSlideGravity = float.Parse(num);
        PlayerPrefs.SetFloat("WallSlideGravity", WallSlideGravity);

    }
    public void Exitt()
    {
        CompleteMenu.gameObject.SetActive(false);
        
    }

}
