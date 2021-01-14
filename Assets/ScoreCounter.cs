using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{

    private BoxCollider col;
    private int count;
    [SerializeField]
    private CapsuleObject.Belong_State myteam;
    [SerializeField]
    private ScoreManager scrMgr;

    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<BoxCollider>();
        count = 0;
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Capsule")
        {
            other.GetComponent<CapsuleObject>().state = myteam;
            int num = other.GetComponent<CapsuleObject>().score;
            for (int i = 0; i < num; i++)
            {
                scrMgr.UpdateScore(myteam);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Capsule")
        {
            other.GetComponent<CapsuleObject>().state = myteam;
            int num = other.GetComponent<CapsuleObject>().score;
            for (int i = 0; i < num; i++)
            {
                scrMgr.DecreaseScore(myteam);
            }
        }
    }

}
