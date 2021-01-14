using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TeamScore : MonoBehaviour
{
    public Text timerText;
    private int totalscore;

    void Start()
    {
        totalscore = 0;
        timerText.text = totalscore.ToString("00");
    }

    public void AddScore()
    {
        totalscore++;
        timerText.text = totalscore.ToString("00");
    }

    public void SubScore()
    {
        totalscore--;
        timerText.text = totalscore.ToString("00");
    }

    public void SetScore(int setscore)
    {
        totalscore = setscore;
        timerText.text = totalscore.ToString("00");
    }
}
