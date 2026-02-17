using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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

    //システムコマンド
    [Header("三つあるボタンをアタッチ")]
    [SerializeField] private CommandButton[] commandButtons;
    [Header("コマンドのデータを全てアタッチ")]
    [SerializeField] private List<CommandData> allCommandDatabase;

    //リザルト用のUI
    [Header("Panel_Resultをアタッチ")]
    [SerializeField] private GameObject resultPanel;
    [Header("ランクのText")]
    [SerializeField] private TextMeshProUGUI rankText;
    [Header("コメントのText")]
    [SerializeField] private TextMeshProUGUI commentText;

    [Header("PlanetVisualizerの参照")]
    public PlanetVisualizer visualizer;

    /// <summary>
    /// シングルトンを設定
    /// </summary>
    private void Awake()
    {
        //ゲーム開始時に最初のカードを配る
        SetNextCommands();
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
    /// リトライボタンから呼ぶ関数
    /// </summary>
    public void OnRetryButton()
    {
        // 現在のシーンを読み込み直す（＝リセット）
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
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

        //ターンが進んだら新しいカードを配る
        if (currentTurn <= maxTurn)
        {
            SetNextCommands();
        }

        //画像の更新処理
        if (visualizer != null)
        {
            visualizer.UpdateVisuals(scoreWater, scoreNature, scoreCiv, scoreTemp);
        }

        //画面更新
        UpdateUI();

        //ゲーム終了判定
        if (currentTurn > maxTurn)
        {
            //結果画面をここに出す
            ShowResult();
        }
    }

    /// <summary>
    /// コマンドをランダムにしてボタンにセット
    /// </summary>
    void SetNextCommands()
    {
        // ボタンの数だけ繰り返す
        for (int i = 0; i < commandButtons.Length; i++)
        {
            //ランダムに1枚選ぶ
            int randomIndex = Random.Range(0, allCommandDatabase.Count);
            CommandData pickedData = allCommandDatabase[randomIndex];

            //ボタンにデータを渡す
            commandButtons[i].SetCommand(pickedData);
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
        {
            statusText.text = $"水:{scoreWater}  温:{scoreTemp}\n緑:{scoreNature}  文:{scoreCiv}";

            // DOTweenの演出
            statusText.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 1);

            // スコアアップ音を鳴らす
            //開始時の一ターン鳴らさない
            if (currentTurn > 1)
            {
                AudioManager.Instance.PlaySE(AudioManager.Instance.seScoreUp);
            }
        }
    }

    /// <summary>
    /// 結果発表のロジック
    /// </summary>
    void ShowResult()
    {
        string rankStr = "Unknown";
        string commentStr = "...";

        if (visualizer != null)
        {
            // Visualizerが判定したランクを取得
            rankStr = visualizer.currentRank.ToString();
            // Visualizerが判定したコメントを取得
            commentStr = visualizer.currentComment;
        }

        // 画面を表示
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
            rankText.text = "Rank: " + rankStr;
            commentText.text = commentStr;
        }

    }

}
