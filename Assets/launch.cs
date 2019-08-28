using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class launch : MonoBehaviourPunCallbacks
{
    public GameObject ConnectedScreen;
    public GameObject DisconnectedScreen;
    public List<Button> RoomBtn = new List<Button>();
    public GameObject RoomParent;
    public void Onclick_ConnectBtn()
    {

        PhotonNetwork.ConnectUsingSettings();
        if (PhotonNetwork.IsConnected)
        {
            ConnectedScreen.gameObject.SetActive(true);

        }
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby(TypedLobby.Default);
    }
    public override void OnJoinedLobby()
    {
       
        if(ConnectedScreen)
        {
            ConnectedScreen.gameObject.SetActive(true);

        }


    }
    public override void OnJoinedRoom()
    {

    }
    public override void OnRoomListUpdate(List<RoomInfo> roominfo) {
        int index = roominfo.Count;
        Debug.LogError(index);

        for (int i=0;i<RoomBtn.Count;i++) {
            RoomBtn[i].gameObject.SetActive(false);
        }
        for (int i=0;i<roominfo.Count;i++) {

            Debug.LogError(roominfo[i].Name);
            RoomBtn[i].gameObject.SetActive(true);

            RoomBtn[i].transform.GetChild(0).GetComponent<Text>().text = roominfo[i].Name;
            int k = i;
            RoomBtn[i].onClick.AddListener(() => GetComponent<UIHandler>().onclick_JoinRoom(roominfo[k].Name));
        }


        foreach (RoomInfo list in roominfo) {

        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        if(DisconnectedScreen!=null)
        {
            DisconnectedScreen.SetActive(true);

        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       

    }
}
