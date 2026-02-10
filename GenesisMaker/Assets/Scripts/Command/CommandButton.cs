using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// コマンドのデータをボタンに移す処理
/// </summary>
public class CommandButton : MonoBehaviour
{
    [Header("このボタンのコマンド")]
    public CommandData myCommandData;

    [Header("ボタンのラベル（自動設定用）")]
    [SerializeField] private TextMeshProUGUI buttonLabel;

    /// <summary>
    /// 開始
    /// </summary>
    void Start()
    {
        //開始時ボタンの名前を変更
        if (myCommandData != null && buttonLabel != null)
        {
            buttonLabel.text = myCommandData.commandName;
        }

        // ボタンコンポーネントを取得し、クリック時の動作を登録する
        GetComponent<Button>().onClick.AddListener(OnClickButton);
    }

    /// <summary>
    /// クリックしたら呼ばれる処理
    /// </summary>
    void OnClickButton()
    {
        if (myCommandData == null) return;

        //GameManageにデータを送る
        GameManager.Instance.ExecuteCommand(myCommandData);
    }
}
