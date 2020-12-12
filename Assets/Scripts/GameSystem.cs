using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameSystem : MonoBehaviour
{
    // スタートボタンが押されたとき
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

//　ゲーム終了ボタンを押したら実行する
    public void EndGame()
        {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
    		Application.Quit();
    #endif
        }

}
