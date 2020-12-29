using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ClientManager : MonoBehaviourPunCallbacks
{
    [SerializeField]
    private GameObject basePlayer;
    private GameObject insPlayer;
    private TypedLobby lob;
    private RoomOptions roomOp;

    [SerializeField]
    private Material mat;
    private PhotonView pv;
    private Rigidbody rig;
    private GameObject[] others;

    [SerializeField]
    private GameObject Panel;
    [SerializeField]
    private Text status;
    [SerializeField]
    private Text statusNum;

    private string UserID;


    [PunRPC]
    void InstantiatePlayer()
    {

        GameObject insPlayer = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-5.0f, 5.0f), 2.0f, Random.Range(-5.0f, 5.0f)), new Quaternion());
        Camera curCam = Camera.current;
        Camera plyCam = insPlayer.GetComponentInChildren<Camera>();
        curCam.enabled = false;
        Destroy(curCam.gameObject);
        plyCam.enabled = true;
        pv = insPlayer.GetComponent<PhotonView>();
        rig = insPlayer.GetComponent<Rigidbody>();
        Destroy(Panel);
        Destroy(status.gameObject);
        Destroy(statusNum.gameObject);

    }

    [PunRPC]
    void JoinRoom()
    {
        PhotonNetwork.JoinRandomRoom();

    }



    // Start is called before the first frame update
    void Start()
    {

        //PhotonNetwork.ConnectToRegion("jp");
        //bool isConnected = PhotonNetwork.ConnectUsingSettings();
        PhotonNetwork.AuthValues = new AuthenticationValues("fun2");
        bool isConnected = PhotonNetwork.ConnectToMaster("18.183.186.148", 5055, "0");


        //bool isConnected = PhotonNetwork.ConnectToMaster("218.110.167.198", 4530, "0");
        //bool isConnected = PhotonNetwork.ConnectToMaster("192.168.1.5", 4530, "0");
        //LoadBalancingClient client = PhotonNetwork.NetworkingClient;
        //client.ConnectToMasterServer();
        //PhotonNetwork.ConnectToMaster("18.183.239.71", 4530, "1ffb49d8-8f8d-407a-9ed6-c4ae6a3c569f");

    }

    void OnGUI()
    {
        //ログインの状態を画面上に出力
        //GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
        //GUILayout.Label(PhotonNetwork.CountOfPlayers.ToString());
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        /*
        foreach (var r in roomList)
        {
            Debug.Log(r.Name + "/MaxPlayers:" + r.MaxPlayers);
            Debug.Log(r.IsOpen);
            Debug.Log(r.IsVisible);
            PhotonNetwork.JoinRoom(r.Name);
        }
        */
    }



    private void Update()
    {
        statusNum.text = PhotonNetwork.CountOfPlayersOnMaster + "/16 Players are waiting...";

        /*
        // デバッグ用
        if (Input.GetKeyDown(KeyCode.Return))
        {
            RoomOptions ro = new RoomOptions
            {
                MaxPlayers = 10,
                IsOpen = true,
                IsVisible = true,
                PublishUserId = true
            };

            Player[] player = PhotonNetwork.PlayerList;
            string[] playerName = new string[player.Length];
            for (int i = 0; i < player.Length; i++)
            {
                playerName[i] = player[i].UserId;
                Debug.Log(playerName[i]);
            }

            PhotonNetwork.JoinOrCreateRoom("room", ro, TypedLobby.Default, playerName);
            //PhotonNetwork.CreateRoom("myroom", ro);
            //photonView.RPC("JoinRoom", RpcTarget.Others);

        }
        */
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Debug.Log(PhotonNetwork.CurrentRoom.PlayerCount);
            Dictionary<int, Player> d = PhotonNetwork.CurrentRoom.Players;
            foreach (var a in d)
            {
                Debug.Log(a.Value.UserId);
            }
        }

    }
    // ロビーに入った時に呼び出される
    public override void OnJoinedLobby()
    {

        RoomOptions ro = new RoomOptions
        {
            MaxPlayers = 10,
            IsOpen = true,
            IsVisible = true,
            PublishUserId = true

        };



        PhotonNetwork.JoinOrCreateRoom("room", ro, TypedLobby.Default);

    }



    // マスターサーバーに接続時に呼び出される
    public override void OnConnectedToMaster()
    {

        // ロビーに入る
        bool isJoin = PhotonNetwork.JoinLobby();

        status.text = "Waiting...";

        // "room"という名前のルームに参加する（ルームが無ければ作成してから参加する）
        //PhotonNetwork.JoinOrCreateRoom("room", new RoomOptions(), TypedLobby.Default);

        //
        //PhotonNetwork.SendRate = 10;
        //PhotonNetwork.SerializationRate = 10;

        // ロビーに入る？
        //Debug.Log(PhotonNetwork.ServerAddress);


        if (PhotonNetwork.AuthValues == null)
        {
            Debug.Log("NULLだよ！！！");
        }
        else
            UserID = PhotonNetwork.AuthValues.UserId;
        Debug.Log(UserID);

    }

    // ルームに入室後に呼び出される
    public override void OnJoinedRoom()
    {
        photonView.RPC("InstantiatePlayer", RpcTarget.All);

        Debug.Log(PhotonNetwork.CountOfPlayersInRooms);
        //InstantiatePlayer();
        /*
        if (PhotonNetwork.CurrentRoom.PlayerCount == 1)
        {
            PhotonNetwork.CurrentRoom.MaxPlayers = 4;
        }

        if (PhotonNetwork.CurrentRoom.PlayerCount == 4)
        {
            Debug.Log("召喚");
            photonView.RPC("InstantiatePlayer", RpcTarget.All);
        }
        */
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("入室に失敗\nエラーコード:" + returnCode + "\nメッセージ:" + message);
    }

}
