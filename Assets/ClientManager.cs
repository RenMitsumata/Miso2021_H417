using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;


public class ClientManager : MonoBehaviourPunCallbacks, IOnEventCallback
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

    public enum EventCode
    {
        JoinRoom = 0x01
    };


    void InstantiatePlayer()
    {
        GameObject insPlayer = PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-5.0f, 5.0f), 2.0f, Random.Range(-5.0f, 5.0f)), new Quaternion());

        Camera curCam = Camera.current;
        Camera plyCam = insPlayer.GetComponentInChildren<Camera>();
        curCam.enabled = false;
        Destroy(curCam.gameObject);
        plyCam.enabled = true;
        Destroy(Panel);
        Destroy(status.gameObject);
        Destroy(statusNum.gameObject);
        pv = insPlayer.GetComponent<PhotonView>();
        rig = insPlayer.GetComponent<Rigidbody>();

    }


    [PunRPC]
    public void InstantiateOthers()
    {
        //PhotonNetwork.Instantiate("Player", new Vector3(Random.Range(-5.0f, 5.0f), 2.0f, Random.Range(-5.0f, 5.0f)), new Quaternion());
    }


    // 生成された後、最初のフレームで呼び出される関数（MonoBehaviorクラスのオーバーライド）
    void Start()
    {

        // 認証情報（ユーザーIDなど）を生成
        PhotonNetwork.AuthValues = new AuthenticationValues("fun");

        // マスターサーバーに接続
        bool isConnected = PhotonNetwork.ConnectToMaster("18.183.186.148", 5055, "0");

        if (isConnected == false)
        {
            // 接続できなかったときの処理
        }
    }

    // ルームリストがアップデートされたとき（誰かがロビーに入ってきたときなど）に呼び出されるコールバック関数
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


    // 更新関数（MonoBehaviorクラスのオーバーライド）
    private void Update()
    {
        statusNum.text = PhotonNetwork.CountOfPlayersOnMaster + "/16 Players are waiting...";


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
    // ロビーに入った時に呼び出される関数
    public override void OnJoinedLobby()
    {
        // 入室処理など

        RoomOptions ro = new RoomOptions
        {
            MaxPlayers = 10,
            IsOpen = true,
            IsVisible = true,
            PublishUserId = true

        };



        PhotonNetwork.JoinOrCreateRoom("room", ro, TypedLobby.Default);

    }



    // マスターサーバーに接続時に呼び出される関数
    public override void OnConnectedToMaster()
    {

        // ロビーに入る
        bool isJoin = PhotonNetwork.JoinLobby();

        status.text = "Waiting...";


    }

    // ルームに入室したときに呼び出される関数
    public override void OnJoinedRoom()
    {
        //photonView.RPC("InstantiatePlayer", RpcTarget.All);

        // 自分をスポーン
        InstantiatePlayer();

        //photonView.RPC("InstantiateOthers", RpcTarget.Others);

        // 入室したというイベントを送信
        //RaiseEventOptions raiseEventOptions = new RaiseEventOptions { Receivers = ReceiverGroup.Others };
        //SendOptions sendOptions = new SendOptions { Reliability = true };
        //PhotonNetwork.RaiseEvent((byte)EventCode.JoinRoom, null, raiseEventOptions, sendOptions);

        //Debug.Log(PhotonNetwork.CountOfPlayersInRooms);

    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("入室に失敗\nエラーコード:" + returnCode + "\nメッセージ:" + message);
    }

    // イベントを受信した時の関数
    public void OnEvent(EventData ed)
    {



    }

}
