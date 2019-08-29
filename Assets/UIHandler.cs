using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class UIHandler : MonoBehaviourPunCallbacks
{
    public InputField createRoom;
    public InputField JoinRoom;
  //  public Text instruction;
    public PhotonView pv;
    public int chosenCharacter;
    public GameObject characterSelectUI, wait_player, gameStartButton;
    //public List<Button> characterSelectButtons;
    public int PlayerCount;
    // Start is called before the first frame update

    public float BaseSpeed;
    public Text BaseSpeedtxt;

    public float TargetSpeed;
    public float MaxRunSpeed;
    public float JumpSpeed;
    public float GravityMultiplier;
    public float TerminalSpeed;
    public GameObject ControllerMenu;
    public bool EnteredFirst;

    public void Onclick_CreateRoom()
        {
        PhotonNetwork.CreateRoom(createRoom.text,new RoomOptions { MaxPlayers = 4 },null);
        EnteredFirst = true;
        //    pv.RPC("Incr", RpcTarget.AllBufferedViaServer, null);

    }
    [PunRPC]
     void BaseSpeedFunc()
    {
        BaseSpeed = BaseSpeed;
    }

    
    public void BaseSpeedUI(string num)
    {
        BaseSpeed = float.Parse(num);
        BaseSpeedtxt.text = BaseSpeed.ToString();
        pv.RPC("BaseSpeedFunc", RpcTarget.AllBuffered, null);


    }

    [PunRPC]
    public void Controller()
    {

       
        PlayerCount += 1;
     //   StartCoroutine(ControllerRoutine());
       
    }
    IEnumerator ControllerRoutine()
    {
        yield return new WaitForSeconds(1f);
        //if (PlayerCount == 1)
        //{
        //    ControllerMenu.gameObject.SetActive(true);
        //    character1.gameObject.SetActive(false);
        //    character2.gameObject.SetActive(false);

        //}
    }

        public void onclick_JoinRoom()
    {
        PhotonNetwork.JoinRoom(JoinRoom.text,null);

    }
    public override void OnConnectedToMaster()
    {

    }
    public override void OnJoinedRoom()
    {

        //  
        pv.RPC("Controller", RpcTarget.AllBuffered, null);

        createRoom.gameObject.SetActive(false);
         JoinRoom.gameObject.SetActive(false);
        //  character1.gameObject.SetActive(true);
        if (EnteredFirst) {
            LevelSelectUI.gameObject.SetActive(true);
        }
        else {
            ActivateCharacterSelectUI(true);
        }


        //  StartCoroutine(waiting());
        //char1();
        if (pv.IsMine)
        {
            
          //  pv.RPC("char1", RpcTarget.AllBuffered, null);

        }



    }
    public int i = 0;
    public GameObject LevelSelectUI;
    public List<Button> LevelBtn = new List<Button>();
    public List<int> LevelSelectedCount = new List<int>();
    public int selectedLevel = 0;
    public void LevelSelectBtnListen(int temp)
    {
        pv.RPC("levelSelectSync", RpcTarget.AllBuffered, temp);

        if (pv.IsMine)
        {
            //  selectedLevel = temp;


            //   pv.RPC("levelSelectSync ", RpcTarget.AllBuffered, null);

        }

        // LevelSelectedCount[temp] += 1;
        for (int i = 0; i < LevelBtn.Count; i++)
        {
            LevelBtn[i].interactable = false;

        }
      //  pv.RPC("Incr", RpcTarget.AllBuffered, null);

        //   pv.RPC("char1", RpcTarget.AllBuffered, null);

    }

    void ActivateCharacterSelectUI(bool setActive) {
        characterSelectUI.SetActive(setActive);

        //if (setActive) {
        //    for (int c = 0; c < characterSelectButtons.Count; c++) {
        //        characterSelectButtons[c].onClick.RemoveAllListeners();
        //        int setIndex = c;
        //        characterSelectButtons[c].onClick.AddListener(() => SelectCharacter(setIndex));
        //    }
        //}
    }


    [PunRPC]
    public void levelSelectSync(int temp)
    {
        //  LevelSelectedCount[temp] += 1;
        selectedLevel = temp;

        Statistics.stats.Pref("StartTrack" + selectedLevel);
        //for (int i = 0; i < LevelSelectedCount.Count; i++)
        //{
        //    if (selectedLevel < LevelSelectedCount[i])
        //    {
        //        selectedLevel = i;
        //    }
        //}

        LevelSelectUI.gameObject.SetActive(false);
        if (EnteredFirst) ActivateCharacterSelectUI(true);

    }

    [PunRPC]
    public void SelectCharacter(int charIndex) {
        ActivateCharacterSelectUI(false);

        chosenCharacter = charIndex;
        pv.RPC("Incr", RpcTarget.AllBuffered, null);

        wait_player.SetActive(true);
      //  gameStartButton.SetActive(EnteredFirst);
    }

    public void TowardsLobby()
    {
          PhotonNetwork.LeaveRoom();
        Destroy(GameObject.Find("AudioCtrl").gameObject);
      //  Destroy(GameObject.Find("AudioCtrl").gameObject);

        PhotonNetwork.LoadLevel(0);
    }
   public void oNQuit()
    {
        Application.Quit();
    }
    [PunRPC]
    public void LoadLevelToLevel() {

        PhotonNetwork.LoadLevel("0716Level01");

    }
  

    [PunRPC]
    public void Incr()
    {
        i++;
        if(i==PlayerCount)
        {
            PhotonNetwork.LoadLevel("0716Level01");
        }
        else
        {
        }
        
    }
    [PunRPC]
    public void Waitfunc()
    {
        StartCoroutine(Wait());

    }
    public IEnumerator Wait()
    {
       
     //   instruction.text = "Connected To The Room \n Room Name-" + createRoom.text;
        createRoom.gameObject.SetActive(false);
        JoinRoom.gameObject.SetActive(false);
        yield return new WaitForSeconds(1f);
      //  instruction.text = "3";
        yield return new WaitForSeconds(1f);

    //    instruction.text = "2";
        yield return new WaitForSeconds(1f);

      //  instruction.text = "1";
        PhotonNetwork.LoadLevel("0716Level01");

    }
    public Vector3 inputPos;
    private void Update()
    {
        inputPos = Input.mousePosition;
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
       
    }
    void Start()
    {
        

        DontDestroyOnLoad(this.gameObject);
    }

   


}
