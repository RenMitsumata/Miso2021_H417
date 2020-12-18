using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private GameObject Capsulepos;

    private GameObject curCapsule;
    private bool isCarry = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (curCapsule == null)
        {
            return;
        }
        if ((Input.GetKeyDown(KeyCode.X) || Input.GetButtonDown("Action")) && !isCarry)
        {
            curCapsule.transform.SetParent(Capsulepos.transform);
            Rigidbody rig = curCapsule.GetComponent<Rigidbody>();
            //curCapsule.GetComponent<CapsuleCollider>().enabled = false;
            curCapsule.GetComponent<CapsuleCollider>().isTrigger = true;
            curCapsule.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);
            rig.velocity = Vector3.zero;
            rig.useGravity = false;
            isCarry = true;
        }
        else if ((Input.GetKeyUp(KeyCode.X) || Input.GetButtonUp("Action")) && isCarry)
        {
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
}
