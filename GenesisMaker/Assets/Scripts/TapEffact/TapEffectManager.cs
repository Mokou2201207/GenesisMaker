using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; 
/// <summary>
/// タップした際に流れるエフェクト
/// </summary>
public class TapEffectManager : MonoBehaviour
{
    [Header("波紋のプレハブ")]
    [SerializeField] private GameObject ripplePrefab;

    [Header("エフェクトを出す親（Canvas）")]
    [SerializeField] private Transform canvasTransform;

    [Header("波紋が広がるサイズ")]
    [SerializeField] private float targetScale = 2f;

    [Header("波紋が消えるまでの秒数")]
    [SerializeField] private float duration = 0.4f;

    void Update()
    {
        // スマホのタップ、またはPCのマウスクリックを感知
        if (Input.GetMouseButtonDown(0))
        {
            CreateRipple(Input.mousePosition);
        }
    }

    /// <summary>
    /// タップのエフェクトの処理
    /// </summary>
    /// <param name="tapPos"></param>
    void CreateRipple(Vector2 tapPos)
    {
        if (ripplePrefab == null || canvasTransform == null) return;

        // プレハブをCanvasの中に生成
        GameObject ripple = Instantiate(ripplePrefab, canvasTransform);

        //位置をタップした場所に合わせる
        ripple.transform.position = tapPos;

        //最初は少し小さくしておく
        ripple.transform.localScale = Vector3.one * 0.2f;

        // Imageコンポーネントを取得
        Image img = ripple.GetComponent<Image>();
        if (img != null)
        {
            // サイズを大きくしながら...
            ripple.transform.DOScale(targetScale, duration).SetEase(Ease.OutQuad);

            // 同時に透明度（アルファ値）を0（透明）にしていく
            img.DOFade(0f, duration).SetEase(Ease.OutQuad)
               .OnComplete(() =>
               {
                   // アニメーションが完全に終わったら、この画像を削除
                   Destroy(ripple);
               });
        }
    }
}