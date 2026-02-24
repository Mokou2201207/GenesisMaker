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

    //リロールボタンのコマンド
    [Header("リロールボタン")]
    [SerializeField] private UnityEngine.UI.Button rerollButton;
    [Header("残り回数表示用のテキスト")]
    [SerializeField] private TMPro.TextMeshProUGUI rerollText; 
    private int rerollCount = 3;
    [Header("リロールボタンの画像")]
    [SerializeField] private UnityEngine.UI.Image rerollButtonImage;
    [Header("使用不可時のスプライト")]
    [SerializeField] private Sprite rerollDisabledSprite;

    //記憶しとく用の変数
    private Sprite rerollNormalSprite;

    //システムコマンド
    [Header("三つあるボタンをアタッチ")]
    [SerializeField] private CommandButton[] commandButtons;
    [Header("コマンドのデータを全てアタッチ")]
    [SerializeField] private List<CommandData> allCommandDatabase;

    //確認ポップアップUI
    [Header("確認ポップアップUI")]
    [SerializeField] private GameObject confirmPopupPanel;
    [Header("「〇〇を使いますか？」のテキスト")]
    [SerializeField] private TextMeshProUGUI confirmTitleText;  
    [Header("ステータス表示テキスト")]
    [SerializeField] private TextMeshProUGUI confirmStatusText;

    [Header("ステータスを表示するパネル")]
    [SerializeField] private GameObject statusInfoPanel;
    [Header("ステータスのテキスト")]
    [SerializeField] private TextMeshProUGUI statusInfoText;  

    // 選んだコマンドを一時的に覚えておくための変数
    private CommandData pendingCommand;

    //保留画面UI（スワイプの方）
    [Header("保留したコマンドを入れておく箱")]
    public List<CommandData> holdCommands = new List<CommandData>();
    [Header("全体のパネル")]
    [SerializeField] private GameObject holdViewPanel;       
    [Header("タイトルテキスト")]
    [SerializeField] private TextMeshProUGUI holdTitleText;   
    [Header("ステータス表示テキスト")]
    [SerializeField] private TextMeshProUGUI holdStatusText;  
    [Header("ページ数テキスト")]
    [SerializeField] private TextMeshProUGUI holdPageText;

    // 今見ている保留コマンドの番号
    private int currentHoldIndex = 0; 

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
        //最初についてるリロールの画像を通常時として記憶させる
        if (rerollButtonImage!=null)
        {
            rerollNormalSprite=rerollButtonImage.sprite;
        }

        UpdateUI();

        // ゲーム開始時にリロール回数を3にして、UIを更新する
        rerollCount = 3;
        UpdateRerollUI();
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

    /// <summary>
    /// リロールボタンが押された際に呼ばれる処理
    /// </summary>
    public void OnRerollButtonClicked()
    {
        //カウントがまだあるなら
        if (rerollCount>0)
        {
            //カウントを減らす
            rerollCount--;
            //画面の数字とボタンを更新
            UpdateRerollUI();
            //カード（コマンド）を新しく更新
            SetNextCommands();
        }
    }

    /// <summary>
    /// 残り回数テキストとボタンのON/OFFを更新する処理
    /// </summary>
    void UpdateRerollUI()
    {
        if (rerollText != null)
        {
            rerollText.text = "引き直し: " + rerollCount + "回";
        }

        if (rerollButton != null && rerollButtonImage != null)
        {
            // 回数が残っているかどうかの判定
            bool canReroll = (rerollCount > 0);

            // 0回になったらボタンを押せなくする
            rerollButton.interactable = canReroll;

            //回数があれば記憶した通常画像、0回なら黒い画像にする
            rerollButtonImage.sprite = canReroll ? rerollNormalSprite : rerollDisabledSprite;
        }
    }

    /// <summary>
    /// コマンドが選ばれた時に呼ばれる
    /// </summary>
    /// <param name="data"></param>
    public void ShowConfirmPopup(CommandData data)
    {
        // 選ばれたコマンドのデータを一旦キープする
        pendingCommand = data;

        // UIのテキストを書き換える
        if (confirmTitleText != null)
            confirmTitleText.text = $"{data.commandName} のコマンドを使いますか？";

        if (confirmStatusText != null)
            confirmStatusText.text = $"水: {data.waterChange} / 温: {data.tempChange} / 緑: {data.natureChange} / 文: {data.civChange}";

        // ポップアップ画面を表示する
        if (confirmPopupPanel != null)
            confirmPopupPanel.SetActive(true);
    }

    /// <summary>
    /// ポップアップの「使用」ボタンを押した時の処理
    /// </summary>
    public void OnConfirmYes()
    {
        // ポップアップを閉じる
        confirmPopupPanel.SetActive(false); 

        if (pendingCommand != null)
        {
            //コマンドを発動させる
            ExecuteCommand(pendingCommand);
            // 保存しておいたものを初期化
            pendingCommand = null;         
        }
    }

    /// <summary>
    /// ポップアップの「戻る」ボタンを押した時の処理
    /// </summary>
    public void OnConfirmClose()
    {
        confirmPopupPanel.SetActive(false); 
        pendingCommand = null;
    }

    /// <summary>
    /// ポップアップの「保留」ボタンを押した時の処理
    /// </summary>
    public void OnConfirmKeep()
    {
        //3つ以上は保留できないようにする
        if (holdCommands.Count >= 3)
        {
            Debug.Log("<color=red>保留枠がいっぱいです！（最大3つまで）</color>");
            return;
        }
        Debug.Log("<color=yellow>保留しました!");
        confirmPopupPanel.SetActive(false);

        //保存用の変数があれば
        if (pendingCommand!=null)
        {
            holdCommands.Add(pendingCommand);

            //保留した時もターンを消費する（パラメータは増えない）
            currentTurn++;
            if (currentTurn <= maxTurn)
            {
                // 次のターンのカードを配る
                SetNextCommands();
            }
            // ターンの表示を更新
            UpdateUI();

            // もし最終ターンを超えたら結果発表へ
            if (currentTurn > maxTurn)
            {
                ShowResult();
            }
        }
        pendingCommand = null;
    }

    /// <summary>
    /// メニューの「保留」ボタンを押した時に開く処理
    /// </summary>
    public void OpenHoldView()
    {
        currentHoldIndex = 0;
        UpdateHoldView();

        if (holdViewPanel != null) holdViewPanel.SetActive(true);
    }

    /// <summary>
    /// 保留画面の表示を更新する
    /// </summary>
    public void UpdateHoldView()
    {
        // なくなったら閉じる
        if (holdCommands.Count == 0)
        {
            if (holdTitleText != null) holdTitleText.text = "保留しているコマンドはありません";
            if (holdStatusText != null) holdStatusText.text = ""; 
            if (holdPageText != null) holdPageText.text = "0 / 0";
            return;
        }

        CommandData data = holdCommands[currentHoldIndex];

        if (holdTitleText != null) holdTitleText.text = $"{data.commandName} のコマンド";
        if (holdStatusText != null) holdStatusText.text = $"水: {data.waterChange} / 温: {data.tempChange} / 緑: {data.natureChange} / 文: {data.civChange}";
        if (holdPageText != null) holdPageText.text = $"{currentHoldIndex + 1} / {holdCommands.Count}";
    }

    /// <summary>
    /// 保留画面の「もどる」ボタン
    /// </summary>
    public void CloseHoldView()
    {
        if (holdViewPanel != null) holdViewPanel.SetActive(false);
    }

    /// <summary>
    /// 保留画面の「使用」ボタン
    /// </summary>
    public void UseHoldCommand()
    {
        if (holdCommands.Count == 0) return;

        CommandData dataToUse = holdCommands[currentHoldIndex]; // 今表示しているデータを取得
        holdCommands.RemoveAt(currentHoldIndex); // リストから消す

        ExecuteCommand(dataToUse); // 発動

        // 残りの保留があるかチェックして画面を更新
        if (currentHoldIndex >= holdCommands.Count && holdCommands.Count > 0)
        {
            currentHoldIndex = holdCommands.Count - 1;
        }
        else if (holdCommands.Count == 0)
        {
            currentHoldIndex = 0; // ゼロ個になったら0ページ目に戻す
        }
        UpdateHoldView();
    }

    // ＝＝＝ ここからスワイプ用の処理 ＝＝＝
    public void HoldNextPage()
    {
        if (currentHoldIndex < holdCommands.Count - 1)
        {
            currentHoldIndex++;
            UpdateHoldView();
        }
    }

    public void HoldPrevPage()
    {
        if (currentHoldIndex > 0)
        {
            currentHoldIndex--;
            UpdateHoldView();
        }
    }

    /// <summary>
    /// コマンドが長押しされた時に呼ばれる（ステータス表示）
    /// </summary>
    public void ShowCommandStatusUI(CommandData data)
    {
        if (statusInfoText != null)
        {
            statusInfoText.text = $"{data.commandName} の効果\n水: {data.waterChange} / 温: {data.tempChange} / 緑: {data.natureChange} / 文: {data.civChange}";
        }
        if (statusInfoPanel != null)
        {
            statusInfoPanel.SetActive(true);
        }
    }

    /// <summary>
    /// 長押しの指を離した時に呼ばれる（ステータス非表示）
    /// </summary>
    public void HideCommandStatusUI()
    {
        if (statusInfoPanel != null)
        {
            statusInfoPanel.SetActive(false);
        }
    }
}
