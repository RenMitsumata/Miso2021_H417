using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreCounter : MonoBehaviour
{

    private BoxCollider col;
    private int count;

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
            count++;
            Debug.Log("カウントアップ");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(other.tag);
        if (other.tag == "Capsule")
        {
            count--;
            Debug.Log("カウントダウン");
        }
    }

}
