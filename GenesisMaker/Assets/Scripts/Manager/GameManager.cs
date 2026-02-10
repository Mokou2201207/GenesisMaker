using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/// <summary>
/// 現在の世界のステータスのマネージャー
/// </summary>
public class GameManager : MonoBehaviour
{
    //シングルトン化(どこからでもアクセス可能)
    public static GameManager Instance;

    //世界の状態
    [Header("開始ターン")]
    public int currentTurn = 1;
    [Header("Maxのターン")]
    public int maxTurn = 10;

    // パラメータ（合計スコア）
    [Header("水分スコア")]
    public int scoreWater = 0;
    [Header("気温スコア")]
    public int scoreTemp = 0;
    [Header("自然スコア")]
    public int scoreNature = 0;
    [Header("文明スコア")]
    public int scoreCiv = 0;

    [Header("UIへの参照")]
    [SerializeField] private TextMeshProUGUI turnText;
    [SerializeField] private TextMeshProUGUI statusText;

    /// <summary>
    /// シングルトンを設定
    /// </summary>
    private void Awake()
    {
        if (Instance == null) Instance = this;
    }


    /// <summary>
    /// 開始
    /// </summary>
    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// ボタンの処理（神の処理）
    /// </summary>
    /// <param name="data">コマンドごとの設定値</param>
    public void ExecuteCommand(CommandData data)
    {
        //ターンが１０ターンまでいったら処理をしない
        if (currentTurn > maxTurn) return;

        //パラメータの加算
        scoreWater += data.waterChange;
        scoreTemp += data.tempChange;
        scoreNature += data.natureChange;
        scoreCiv += data.civChange;

        //ログ出し
        Debug.Log($"<color=cyan>コマンド実行: {data.commandName}</color>");
        Debug.Log($"現在の状態 -> 水:{scoreWater} 温:{scoreTemp} 緑:{scoreNature} 文:{scoreCiv}");

        //ターンの経過
        currentTurn++;

        //画面更新
        UpdateUI();

        // 4. ゲーム終了判定
        if (currentTurn > maxTurn)
        {
            Debug.Log("<color=yellow>ゲーム終了！リザルト画面へ！</color>");
            // ここにリザルト遷移処理を後で書く
        }
    }

   /// <summary>
   /// UIテキストの更新（ターン、ステータス）
   /// </summary>
    void UpdateUI()
    {
        if (turnText != null)
            turnText.text = $"Turn {currentTurn} / {maxTurn}";

        if (statusText != null)
            statusText.text = $"水:{scoreWater}  温:{scoreTemp}\n緑:{scoreNature}  文:{scoreCiv}";
    }
}
