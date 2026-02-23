using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
/// <summary>
/// 長押ししてスライドをした際のエフェクト
/// </summary>
public class TrailEffectManager : MonoBehaviour
{
    [Header("光の粒のプレハブ")]
    [SerializeField] private GameObject trailDotPrefab;

    [Header("エフェクトを出す親（Canvas）")]
    [SerializeField] private Transform canvasTransform;

    [Header("粒が出る間隔（距離）※小さいほど密になる")]
    [SerializeField] private float spawnDistance = 15f;

    [Header("粒が消えるまでの時間（秒）")]
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("星が出始めるまでの遅延時間（秒）")]
    [SerializeField] private float delayTime = 1.0f;

    // 最後に粒を出した場所
    private Vector2 lastSpawnPos; 
    private bool isDragging = false;
    // タッチし始めた時間を記録する変数
    private float touchStartTime = 0f; 

    void Update()
    {
        // 画面に触れた瞬間
        if (Input.GetMouseButtonDown(0))
        {
            lastSpawnPos = Input.mousePosition;
            isDragging = true;
            touchStartTime = Time.time; 
        }

        // 画面を押している間（スライド中）
        if (Input.GetMouseButton(0) && isDragging)
        {
            // タッチしてからの時間が、設定した遅延時間を超えたかチェック
            if (Time.time >= touchStartTime + delayTime)
            {
                // 1秒経ったので、距離を計算して星を出す
                float distance = Vector2.Distance(Input.mousePosition, lastSpawnPos);

                if (distance >= spawnDistance)
                {
                    SpawnDot(Input.mousePosition);
                    lastSpawnPos = Input.mousePosition; // 場所を更新
                }
            }
            else
            {
                lastSpawnPos = Input.mousePosition;
            }
        }

        // 指を離した時
        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
        }
    }

    // 粒を1つ出して、フェードアウトさせる処理
    void SpawnDot(Vector2 pos)
    {
        if (trailDotPrefab == null || canvasTransform == null) return;

        GameObject dot = Instantiate(trailDotPrefab, canvasTransform);
        dot.transform.position = pos;

        dot.transform.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));

        Image img = dot.GetComponent<Image>();
        if (img != null)
        {
            float rotateAngle = Random.Range(90f, 180f) * (Random.value > 0.5f ? 1 : -1);
            dot.transform.DORotate(new Vector3(0f, 0f, rotateAngle), fadeDuration, RotateMode.LocalAxisAdd).SetEase(Ease.Linear);

            img.DOFade(0f, fadeDuration).SetEase(Ease.Linear)
               .OnComplete(() =>
               {
                   Destroy(dot);
               });
        }
    }
}