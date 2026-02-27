using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
/// <summary>
/// UIの処理のマネージャー
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("終了パネル")]
    [SerializeField] private GameObject quitConfirmationPanel;

    [Header("タイトルパネル")]
    [SerializeField] private GameObject gotitlePanel;

    private void Start()
    {
        // ゲーム開始時はパネルを非表示にしておく
        if (quitConfirmationPanel != null)
        {
            quitConfirmationPanel.SetActive(false);
            gotitlePanel.SetActive(false);
        }
    }

    // ホームボタンを押したとき（パネルを表示する）
    public void ShowQuitPanel()
    {
        // SE
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seButton);
        }

        quitConfirmationPanel.SetActive(true);
    }

    // 「戻る」ボタンを押したとき（パネルを閉じる）
    public void HideQuitPanel()
    {
        // SE
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seButton);
        }

        if (quitConfirmationPanel.activeSelf)
        quitConfirmationPanel.SetActive(false);
        if (gotitlePanel.activeSelf)
            gotitlePanel.SetActive(false);
    }

    // 「終了」ボタンを押したとき
    public void ExecuteQuit()
    {
        // SE
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seButton);
        }

        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // エディタ上での終了用
        #endif
    }

    // タイトルボタンを押したとき（パネルを表示する）
    public void GoTitlePanel()
    {
        // SE
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seButton);
        }

        gotitlePanel.SetActive(true);
    }

    // 「タイトルへ」ボタンを押したとき
    public void TitleQuit()
    {
        // SE
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(AudioManager.Instance.seButton);
        }

        //タイトルシーンへ
        SceneManager.LoadScene("Title");
    }
}
