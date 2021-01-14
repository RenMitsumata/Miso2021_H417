using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    private int[] scores;
    [SerializeField]
    private DebugSceneUpdate dsu;
    [SerializeField]
    private TeamScore Red, Green, Blue, Yellow;

    // Start is called before the first frame update
    void Start()
    {
        scores = new int[4];
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScore(CapsuleObject.Belong_State team)
    {
        switch (team)
        {
            case CapsuleObject.Belong_State.Red:
                Red.AddScore();
                break;

            case CapsuleObject.Belong_State.Green:
                Green.AddScore();
                break;


            case CapsuleObject.Belong_State.Blue:
                Blue.AddScore();
                break;

            case CapsuleObject.Belong_State.Yellow:
                Yellow.AddScore();
                break;

        }
    }

}
