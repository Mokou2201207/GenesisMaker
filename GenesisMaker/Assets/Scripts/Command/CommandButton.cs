using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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
        // ボタンコンポーネントを取得し、クリック時の動作を登録する
        GetComponent<Button>().onClick.AddListener(OnClickButton);
    }

    /// <summary>
    /// コマンドの名前やステータスが変わる処理
    /// </summary>
    /// <param name="newDate">コマンドのステータス</param>
    public void SetCommand(CommandData newData)
    {
        myCommandData = newData;

        // 見た目の更新
        if (buttonLabel != null && myCommandData != null)
        {
            buttonLabel.text = myCommandData.commandName;
        }

        //サイズをゼロに
        transform.localScale = Vector3.zero;
        //0.5秒にかけてサイズを１に戻す（弾力を表現）
        transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack);
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
