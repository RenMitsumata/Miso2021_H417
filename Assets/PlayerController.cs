﻿using System.Collections;
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

    [SerializeField]
    private float jumpPow = 9.8f;

    private float h;
    private float v;
    private Vector3 velocity;
    private bool isJumping = false;
    private bool isFalling = false;
    private bool isLanding = false;
    private ButtonStatus[] Buttons;

    private Animator animator;
    private AnimatorStateInfo animatorStateInfoBase;
    private AnimatorStateInfo animatorStateInfoCarry;
    private Rigidbody rb;
    private AudioSource audioSource;
    private SoundManager sm;
    private int hitTimer = -1;

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
        AudioClip ac = sm.GetAudioClip("Pick");    // SoundManagerから鳴らしたいAudioClipを取得
        audioSource.clip = ac;                      // AudioSourceに鳴らしたいAudioClipをセット
        audioSource.Play();                         // 再生
    }


    [PunRPC]
    public void ThrowCapsule(bool isSmashed)
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
        if (!isSmashed)
        {
            AudioClip ac = sm.GetAudioClip("Throw");    // SoundManagerから鳴らしたいAudioClipを取得
            audioSource.clip = ac;                      // AudioSourceに鳴らしたいAudioClipをセット
            audioSource.Play();                         // 再生
        }

    }





    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        Buttons = new ButtonStatus[dic.Count];
        audioSource = GetComponent<AudioSource>();
        sm = GameObject.Find("SoundManager").GetComponent<SoundManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isMine)
        {
            return;
        }



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

        // トリガー用変数をリセット
        animator.SetBool("Run", false);
        animator.SetBool("Back", false);
        animator.SetBool("Idle", false);
        animator.SetBool("Punch", false);


        string animName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        Debug.Log(animName);

        if (animName == "Idle" && v > 0.1f && !animator.IsInTransition(0))
        {
            animator.SetBool("Run", true);
        }

        else if (animName == "Idle" && v < -0.1f && !animator.IsInTransition(0))
        {
            animator.SetBool("Back", true);
        }

        else if ((animName == "Running" || animName == "Walking") && Mathf.Abs(v) < 0.1f && !animator.IsInTransition(0))
        {
            animator.SetBool("Idle", true);
        }

        animName = animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;

        // カプセルを取るor投げる
        if (Buttons[0].isTrigger)
        {
            if (isCarry)
            {
                GetComponent<PhotonView>().RPC("ThrowCapsule", RpcTarget.All, false);

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
        if (Buttons[1].isTrigger && !animator.GetCurrentAnimatorStateInfo(1).IsName("Punch") && !animator.IsInTransition(1) && !isCarry)
        {
            animator.SetBool("Punch", true);

            // 音を鳴らす
            AudioClip ac = sm.GetAudioClip("Punch");    // SoundManagerから鳴らしたいAudioClipを取得
            audioSource.clip = ac;                      // AudioSourceに鳴らしたいAudioClipをセット
            audioSource.Play();                         // 再生

            if (enemy)
            {
                hitTimer = 30;

            }
        }

        if (hitTimer > 0)
        {
            hitTimer--;
            if (hitTimer == 0)
            {
                AudioClip acHit = sm.GetAudioClip("Hit");   // SoundManagerから鳴らしたいAudioClipを取得
                audioSource.clip = acHit;                   // AudioSourceに鳴らしたいAudioClipをセット
                audioSource.Play();                         // 再生
                enemy.GetComponent<PhotonView>().RPC("ThrowCapsule", RpcTarget.All, true);
                hitTimer = -1;
            }
        }

        // ジャンプ
        if (Buttons[2].isTrigger && !isJumping)
        {
            if (Mathf.Abs(rb.velocity.y) < 0.3f)
            {
                rb.AddForce(new Vector3(0.0f, jumpPow * 30, 0.0f), ForceMode.Acceleration);
                isJumping = true;
                //animator.SetBool("Landing", false);
                animator.SetBool("Jump", true);
                AudioClip acHit = sm.GetAudioClip("Jump");   // SoundManagerから鳴らしたいAudioClipを取得
                audioSource.clip = acHit;                   // AudioSourceに鳴らしたいAudioClipをセット
                audioSource.Play();                         // 再生
            }
            return;
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
        if (v < 0.0f)
        {
            v *= 0.5f;
        }
        velocity = new Vector3(0, 0, v * 3);        // 上下のキー入力からZ軸方向の移動量を取得


        // キャラクターのローカル空間での方向に変換
        velocity = transform.TransformDirection(velocity);


        // 上下のキー入力でキャラクターを移動させる
        transform.localPosition += velocity * Time.fixedDeltaTime;

        // 左右のキー入力でキャラクタをY軸で旋回させる
        transform.Rotate(0, h * 2, 0);


        //rb.AddForce(10 * Physics.gravity, ForceMode.Acceleration);
        //transform.Translate(transform.forward * Time.fixedDeltaTime);



        // ジャンプボタンが押されたら
        if (rb.velocity.y < -0.2f && !isFalling && isJumping)
        {

            //animator.SetBool("Landing", true);
            isFalling = true;

            return;

        }

        if (isFalling && Mathf.Abs(rb.velocity.y) < 0.0001f)
        {

            isFalling = false;

            isJumping = false;
            animator.SetBool("Jump", false);
            AudioClip acHit = sm.GetAudioClip("Landing");   // SoundManagerから鳴らしたいAudioClipを取得
            audioSource.clip = acHit;                       // AudioSourceに鳴らしたいAudioClipをセット
            audioSource.Play();                             // 再生

        }




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

        if (collision.transform.tag == "Stage")
        {
            isLanding = true;
        }

    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == curCapsule && !isCarry)
        {
            curCapsule = null;
        }
        if (collision.transform.tag == "Stage")
        {
            isLanding = false;
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
        if (other.gameObject == enemy && hitTimer < 0)
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
