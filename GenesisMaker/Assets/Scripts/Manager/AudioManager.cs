using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// SEやBGMを鳴らす処理
/// </summary>
public class AudioManager : MonoBehaviour
{
    //シングルトン化
   public static AudioManager Instance;

    [Header("SEのスピーカー")]
    [SerializeField] private AudioSource seSource;
    [Header("BGMのスピーカー")]
    [SerializeField] private AudioSource bgmSource; 

    [Header("ボタンを押した音")]
    public AudioClip seButton;  
    [Header("スコアが増えた音")]
    public AudioClip seScoreUp; 
    [Header("結果発表の音")]
    public AudioClip seResult;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // シーンが変わっても音が途切れないようにする
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 指定された音を1回鳴らす関数
    /// </summary>
    public void PlaySE(AudioClip clip)
    {
        if (clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }
}
