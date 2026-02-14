using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// EventTriggerで押したときにStart開始
/// </summary>
public class GameStart : MonoBehaviour
{
    /// <summary>
    /// タイトルを押すとゲーム開始
    /// </summary>
    public void TitleStart()
    {
        SceneManager.LoadScene("MainGame");
    }
}
