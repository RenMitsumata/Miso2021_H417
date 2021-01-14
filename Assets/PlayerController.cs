using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviour
{
    struct ButtonStatus
    {
        public bool isTrigger { get; set; }
        public bool isPress { get; set; }
        public bool isRelease { get; set; }
    }



    [SerializeField]
    private List<string> dic;

    [SerializeField]
    private GameObject Capsulepos;

    private GameObject curCapsule;
    private GameObject enemy;
    private bool isCarry = false;
    private bool isMine = false;

    public float forwardSpeed;
    public float backwardSpeed;
    public float rotateSpeed;
    private float h;
    private float v;
    private Vector3 velocity;
    private ButtonStatus[] Buttons;

    private Animator animator;
    private AnimatorStateInfo animatorStateInfoBase;
    private AnimatorStateInfo animatorStateInfoCarry;
    private Rigidbody rb;

    [PunRPC]
    public void CatchCapsule()
    {

        Rigidbody rig = curCapsule.GetComponent<Rigidbody>();
        curCapsule.GetComponent<CapsuleCollider>().isTrigger = true;
        curCapsule.transform.SetParent(Capsulepos.transform);
        curCapsule.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);



        Vector3 curRot = new Vector3(0.0f, 0.0f, 90.0f);
        Quaternion quat = Quaternion.Euler(curRot);
        curCapsule.transform.localRotation = quat;


        rig.velocity = Vector3.zero;
        rig.useGravity = false;
        rig.freezeRotation = true;
        rig.isKinematic = true;
        isCarry = true;
        animator.SetBool("Carry", true);
        animator.SetLayerWeight(1, 0.5f);
    }


    [PunRPC]
    public void ThrowCapsule()
    {
        if (!isCarry)
        {
            return;
        }
        curCapsule.transform.parent = null;
        //curCapsule.GetComponent<CapsuleCollider>().enabled = true;
        curCapsule.GetComponent<CapsuleCollider>().isTrigger = false;
        Rigidbody rig = curCapsule.GetComponent<Rigidbody>();
        Vector3 front = transform.forward * 5;
        rig.velocity = front + new Vector3(0.0f, 5.0f, 0.0f);
        Debug.Log(rig.velocity);
        rig.useGravity = true;
        rig.freezeRotation = false;
        rig.isKinematic = false;
        isCarry = false;
        curCapsule = null;
        animator.SetBool("Carry", false);
        animator.SetLayerWeight(1, 1.0f);
    }





    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Buttons = new ButtonStatus[dic.Count];
    }

    // Update is called once per frame
    void Update()
    {
        // 入力軸の取得
        h = Input.GetAxis("Horizontal");              // 入力デバイスの水平軸をhで定義
        v = Input.GetAxis("Vertical");                // 入力デバイスの垂直軸をvで定義

        // 入力ボタンの取得
        for (int i = 0; i < dic.Count; ++i)
        {
            Buttons[i].isTrigger = Input.GetButtonDown(dic[i]);
            Buttons[i].isPress = Input.GetButton(dic[i]);
            Buttons[i].isRelease = Input.GetButtonUp(dic[i]);
        }

        animatorStateInfoBase = animator.GetCurrentAnimatorStateInfo(0);
        animatorStateInfoCarry = animator.GetCurrentAnimatorStateInfo(1);

        // ここからアニメーション関係
        string animName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        Debug.Log(animName);

        if (animName == "Idle" && v > 0.1f && !animator.IsInTransition(0))
        {
            animator.SetTrigger("Run");
        }

        else if (animName == "Idle" && v < -0.1f && !animator.IsInTransition(0))
        {
            animator.SetTrigger("Back");
        }

        else if ((animName == "Running" || animName == "Walking") && Mathf.Abs(v) < 0.1f && !animator.IsInTransition(0))
        {
            animator.SetTrigger("Idle");
        }

        animName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;


        // カプセルを取るor投げる
        if (Buttons[0].isTrigger)
        {
            if (isCarry)
            {
                GetComponent<PhotonView>().RPC("ThrowCapsule", RpcTarget.All);

            }
            else
            {
                if (curCapsule == null)
                {
                    return;
                }

                GetComponent<PhotonView>().RPC("CatchCapsule", RpcTarget.All);

            }
        }

        // 殴る
        if (Buttons[1].isTrigger)
        {
            animator.SetTrigger("Punch");
            if (enemy)
            {
                enemy.GetComponent<PhotonView>().RPC("ThrowCapsule", RpcTarget.All);
            }
        }
    }

    private void FixedUpdate()
    {
        if (!rb)
        {
            return;
        }
        if (!isMine)
        {
            return;
        }




        // 以下、キャラクターの移動処理
        velocity = new Vector3(0, 0, v * 3);        // 上下のキー入力からZ軸方向の移動量を取得
                                                    // キャラクターのローカル空間での方向に変換
        velocity = transform.TransformDirection(velocity);


        // 上下のキー入力でキャラクターを移動させる
        transform.localPosition += velocity * Time.fixedDeltaTime;

        // 左右のキー入力でキャラクタをY軸で旋回させる
        transform.Rotate(0, h * 2, 0);


        //rb.AddForce(10 * Physics.gravity, ForceMode.Acceleration);
        //transform.Translate(transform.forward * Time.fixedDeltaTime);







    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isCarry)
        {
            return;
        }

        if (collision.transform.tag == "Capsule")
        {
            curCapsule = collision.gameObject;
        }


    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == curCapsule && !isCarry)
        {
            curCapsule = null;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.transform.tag == "Player")
        {
            enemy = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == enemy)
        {
            enemy = null;
        }
    }

    public void Move()
    {

    }

    public void SetMine()
    {
        isMine = true;
    }
}
