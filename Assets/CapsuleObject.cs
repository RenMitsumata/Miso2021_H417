using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleObject : MonoBehaviour
{
    public enum Belong_State
    {
        Neutral = 0,
        Red,
        Blue,
        Green,
        Yellow
    }

    public int score;
    public Belong_State state { get; set; }

    // Start is called before the first frame update
    void Start()
    {
        state = Belong_State.Neutral;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
