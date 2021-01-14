using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugSceneUpdate : MonoBehaviour
{
    private GameObject RedScore,GreenScore,BlueScore,YellowScore;
    public int Red_SetScore, Green_SetScore, Blue_SetScore, Yellow_SetScore;
    // Start is called before the first frame update
    void Start()
    {
        RedScore = GameObject.Find("RedScore_Text");
        GreenScore = GameObject.Find("GreenScore_Text");
        BlueScore = GameObject.Find("BlueScore_Text");
        YellowScore = GameObject.Find("YellowScore_Text");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            RedScore.GetComponent<TeamScore>().AddScore();
            Debug.Log("赤スコア１追加");
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            GreenScore.GetComponent<TeamScore>().AddScore();
            Debug.Log("緑スコア１追加");
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            BlueScore.GetComponent<TeamScore>().AddScore();
            Debug.Log("青スコア１追加");
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            YellowScore.GetComponent<TeamScore>().AddScore();
            Debug.Log("黄スコア１追加");
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            YellowScore.GetComponent<TeamScore>().SubScore();
            Debug.Log("黄スコア１減少");
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            Yellow_SetScore = 0;
            YellowScore.GetComponent<TeamScore>().SetScore(Yellow_SetScore);
            Debug.Log("黄スコアをセット");
        }
    }
}
