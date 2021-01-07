using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;


public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject Capsulepos;

    private GameObject curCapsule;
    private GameObject enemy;
    private bool isCarry = false;

    public float forwardSpeed;
    public float backwardSpeed;
    public float rotateSpeed;
    private float h;
    private float v;
    private Vector3 velocity;

    [PunRPC]
    public void CatchCapsule()
    {

        Rigidbody rig = curCapsule.GetComponent<Rigidbody>();
        curCapsule.GetComponent<CapsuleCollider>().isTrigger = true;
        curCapsule.transform.SetParent(Capsulepos.transform);
        curCapsule.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
        rig.velocity = Vector3.zero;
        rig.useGravity = false;
        isCarry = true;
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
        isCarry = false;
        curCapsule = null;
    }





    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {


        if ((Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Action")))
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

        if (Input.GetKeyDown(KeyCode.F))
        {
            if (enemy)
            {
                enemy.GetComponent<PhotonView>().RPC("ThrowCapsule", RpcTarget.All);
            }
        }
    }

    private void FixedUpdate()
    {

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
        GetComponent<UnityChan.UnityChanControlScriptWithRgidBody>().isMine = true;
    }



}
