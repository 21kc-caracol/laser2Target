using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using Photon.Pun;
using Photon.Realtime;
#if PLATFORM_ANDROID
using UnityEngine.Android;
#endif


public class main_scene_script : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject controlPanel;

    [SerializeField]
    private Text feedbackText;

    [SerializeField]
    private byte maxPlayersPerRoom = 2;

    bool isConnecting;

    string gameVersion = "1";

    [Space(10)]
    [Header("Custom Variables")]
    public InputField playerNameField;
    public InputField roomNameField;

    [Space(5)]
    public Text playerStatus;
    public Text connectionStatus;
    public Text enterDetails;

    [Space(5)]
    public GameObject roomJoinUI;
    public GameObject buttonLoadArena;
    public GameObject buttonJoinRoom;
    //public GameObject buttonEnterDetails;
    public GameObject appImage;

    string playerName = "";
    string roomName = "";

    // Start Method
    void Start()
    {
        //request permissions
        #if PLATFORM_ANDROID
        if (!Permission.HasUserAuthorizedPermission(Permission.FineLocation))
        {
            Permission.RequestUserPermission(Permission.FineLocation);
        }
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
        }
        #endif


        //1
        PlayerPrefs.DeleteAll();

        Debug.Log("Connecting to Photon Network");

        //2
        roomJoinUI.SetActive(false);
        buttonLoadArena.SetActive(false);

        //3
        ConnectToPhoton();
    }

    void Awake()
    {
        //4 
        PhotonNetwork.AutomaticallySyncScene = true;
    }
    // Helper Methods
    // Helper Methods
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    public void SetRoomName(string name)
    {
        roomName = name;
    }

    // Tutorial Methods
    // Tutorial Methods
    void ConnectToPhoton()
    {
        appImage.SetActive(true);
        connectionStatus.text = "Connecting...";
        PhotonNetwork.GameVersion = gameVersion; //1
        PhotonNetwork.ConnectUsingSettings(); //2
    }



    public void JoinRoom()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LocalPlayer.NickName = playerName; //1
            Debug.Log("PhotonNetwork.IsConnected! | Trying to Create/Join Room " +
                roomNameField.text);
            RoomOptions roomOptions = new RoomOptions(); //2
            TypedLobby typedLobby = new TypedLobby(roomName, LobbyType.Default); //3
            PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, typedLobby); //4
        }
    }

    public void LoadArena()
    {
        // 5
        if (PhotonNetwork.CurrentRoom.PlayerCount > 1)
        {
            PhotonNetwork.LoadLevel("Find_scene");
        }
        else
        {
            playerStatus.text = "Minimum 2 Players\nrequired to start!";
        }
    }



    // Photon Methods
    // Photon Methods
    public override void OnConnected()
    {
        // 1
        base.OnConnected();
        // 2
        appImage.SetActive(false);
        enterDetails.text = "Please enter your details";
        enterDetails.color = new Color32(253, 141, 14, 255);
        connectionStatus.text = "";
        connectionStatus.color = Color.green;
        roomJoinUI.SetActive(true);
        buttonLoadArena.SetActive(false);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        // 3
        isConnecting = false;
        controlPanel.SetActive(true); //lev according to tutorial this should be set to false(?)
        Debug.LogError("Disconnected. Please check your Internet connection.");
    }

    public override void OnJoinedRoom()
    {
        // 4
        if (PhotonNetwork.IsMasterClient)
        {
            buttonLoadArena.SetActive(true);
            buttonJoinRoom.SetActive(false);
            playerStatus.text = "You are the TARGET\n waiting for second player to join";
            enterDetails.color = Color.clear;
        }
        else
        {
            buttonJoinRoom.SetActive(false);
            playerStatus.text = "You are the LASER\n waiting for second player to start";
            enterDetails.color = Color.clear;
        }
    }

}
