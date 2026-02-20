using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
/// <summary>
/// コマンドのデータをボタンに移す処理
/// </summary>
public class CommandButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("このボタンのコマンド")]
    public CommandData myCommandData;

    [Header("ボタンのラベル（自動設定用）")]
    [SerializeField] private TextMeshProUGUI buttonLabel;

    //今ボタンを押しているか
    private bool isPressing = false;
    // 長押し状態か
    private bool isLongPress = false;
    // 押している時間
    private float pressTime = 0f;
    // 何秒押したら「長押し」にするか
    private float longPressThreshold = 0.5f;

    private void Update()
    {
        //ボタンを押してくれる間時間を測る
        if (isPressing)
        {
            pressTime += Time.deltaTime;

            // もし設定した時間を超えて、まだ長押し状態じゃなかったら
            if (pressTime >= longPressThreshold && !isLongPress)
            {
                // 長押し判定ON！
                isLongPress = true; 
                GameManager.Instance.ShowCommandStatusUI(myCommandData);
            }
        }
    }

    /// <summary>
    /// ボタンを「押した瞬間」に呼ばれる
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressing = true;
        isLongPress = false;
        pressTime = 0f;

        // 押した時に少し沈み込む演出
        transform.DOScale(1.8f, 0.1f);
    }

    /// <summary>
    /// ボタンから「指を離した瞬間」に呼ばれる
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressing = false;
        // 大きさを元に戻す
        transform.DOScale(2f, 0.1f); 

        if (isLongPress)
        {
            // 長押しだった場合：指を離したので、ステータス表示を消す
            GameManager.Instance.HideCommandStatusUI();
        }
        else
        {
            // 長押しじゃなかった場合：コマンドを発動
            if (myCommandData != null)
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.seButton);
                GameManager.Instance.ShowConfirmPopup(myCommandData);
            }
        }
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
        transform.DOScale(2f, 0.5f).SetEase(Ease.OutBack);
    }
}
