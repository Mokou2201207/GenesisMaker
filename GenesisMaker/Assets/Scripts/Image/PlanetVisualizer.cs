using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// ランクの種類を定義
/// </summary>
public enum PlanetRank
{
    S,
    A,
    B,
    C,
    D,
    E,
    F,
}

/// <summary>
/// 画像Change
/// </summary>
public class PlanetVisualizer : MonoBehaviour
{
    [Header("表示先のImage")]
    [SerializeField] private Image targetImage;

    [Header("条件リスト（上にあるものが優先される）")]
    public List<PlanetState> planetStates = new List<PlanetState>();

    [Header("デフォルトの画像（どれにも当てはまらない時）")]
    [SerializeField] private Sprite defaultSprite;

    [Header(" デフォルトのランク")]
    [SerializeField] private PlanetRank defaultRank = PlanetRank.F;
    [Header("デフォルトのコメント")]
    [SerializeField] private string defaultComment = "未知の惑星...";

    [Header("チャット用のデフォルトテキスト")]
    [SerializeField] private string defaultChatComment = "星に変化が起きたようだ...";

    //コメント
    [Header("チャットのText")]
    [SerializeField] private Text chatText;

    [Header("文字が出るスピード")]
    [SerializeField] private float typeSpeed = 0.05f;

    [Header("画面操作ブロック用の透明パネル")]
    [SerializeField] private GameObject inputBlockerPanel;

    //外部から今の状態を見れるようにする変数
    public PlanetRank currentRank { get; private set; }
    public string currentComment { get; private set; }

    // 画像のキャッシュ
    private Sprite currentSprite;

    private Coroutine typingCoroutine;

    /// <summary>
    /// ステータスを受け取って、リストを上から順にチェックする
    /// </summary>
    public void UpdateVisuals(int water, int nature, int civ, int temp)
    {
        foreach (var state in planetStates)
        {
            if (state.IsMatch(water, nature, civ, temp))
            {
                // マッチした条件のランクとコメントを保存する
                currentRank = state.rank;
                currentComment = state.comment;

                // マッチしたら画像とコメントの変更処理へ
                ChangeState(state.sprite, state.chatComment);
                return;
            }
        }

        // 全部チェックしてダメだったら、デフォルトにする
        currentRank = defaultRank;
        currentComment = defaultComment;
        ChangeState(defaultSprite, defaultComment);
    }

    /// <summary>
    /// 画像を変更し、必要ならコメントを流す処理
    /// </summary>
    void ChangeState(Sprite newSprite, string newComment)
    {
        if (newSprite == null || targetImage == null) return;

        //画像が同じなら処理しない
        if (currentSprite == newSprite) return;

        // 画像を新しいものに更新
        targetImage.sprite = newSprite;
        currentSprite = newSprite;

        // 前の文字送り演出が動いていたら止める
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
        }

        // 新しいコメントの文字送り演出をスタート
        typingCoroutine = StartCoroutine(TypeTextCoroutine(newComment));
    }

    /// <summary>
    /// 1文字ずつ表示する機能
    /// </summary>
    IEnumerator TypeTextCoroutine(string textToType)
    {
        // コマンドを押せなくする（透明パネルをON）
        if (inputBlockerPanel != null) inputBlockerPanel.SetActive(true);

        // テキストを一旦空にする
        if (chatText != null) chatText.text = "";

        // 1文字ずつ追加していく
        if (chatText != null)
        {
            foreach (char c in textToType.ToCharArray())
            {
                chatText.text += c;

                //コメント中SEを再生
                if (AudioManager.Instance != null && c != ' ' && c != '　')
                {
                    AudioManager.Instance.PlaySE(AudioManager.Instance.seTyping);
                }

                // 指定したスピード分待つ
                yield return new WaitForSeconds(typeSpeed);
            }
        }

        //文字が全部出終わったら、コマンドを押せるようにする（透明パネルをOFF）
        if (inputBlockerPanel != null) inputBlockerPanel.SetActive(false);
    }
}

/// <summary>
/// リストの中身
/// </summary>
/// ↓これを書くとInspectorに表示される
[System.Serializable]
public class PlanetState
{
    [Header("画像の名前")]
    public string name;

    [Header("表示するスプライト")]
    public Sprite sprite;

    [Header("このランク")]
    public PlanetRank rank;

    [Header("リザルト時のコメント")]
    [TextArea] public string comment;

    [Header("変化時にチャットに出るテキスト")]
    [TextArea] public string chatComment;

    [Header("発生条件（Min以上 ～ Max以下）")]
    [Header("水のステータス")]
    [Range(-999, 999)] public int minWater = 0;
    [Range(-999, 999)] public int maxWater = 999;

    [Header("自然のステータス")]
    [Range(-999, 999)] public int minNature = 0;
    [Range(-999, 999)] public int maxNature = 999;

    [Header("文明のステータス")]
    [Range(-999, 999)] public int minCiv = 0;
    [Range(-999, 999)] public int maxCiv = 999;

    [Header("温度のステータス")]
    [Range(-999, 999)] public int minTemp = 0;
    [Range(-999, 999)] public int maxTemp = 999;

    // 条件判定をする機能
    public bool IsMatch(int w, int n, int c, int t)
    {
        // 全ての条件が範囲内に入っているか？
        bool isWaterOk = (w >= minWater && w <= maxWater);
        bool isNatureOk = (n >= minNature && n <= maxNature);
        bool isCivOk = (c >= minCiv && c <= maxCiv);
        bool isTempOk = (t >= minTemp && t <= maxTemp);

        // 全部OKなら true を返す
        return isWaterOk && isNatureOk && isCivOk && isTempOk;
    }
}