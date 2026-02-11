using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// 数値をチェックして、適切な画像を貼る
/// </summary>
public class PlanetVisualizer : MonoBehaviour
{
    [Header("Mein画像をアタッチ（色々な画像に変わる場所）")]
    [SerializeField] private Image targetImage;

    //画像の素材
    [Header("荒野")]
    [SerializeField] private Sprite stage0_Wasteland;
    [Header("海")]
    [SerializeField] private Sprite stage1_Water;    
    [Header("森")]
    [SerializeField] private Sprite stage2_Nature;    
    [Header("都市")]
    [SerializeField] private Sprite stage3_Civil;    

    /// <summary>
    /// 数値を受け取って、画像を着せ替える関数
    /// </summary>
    public void UpdateVisuals(int water, int nature, int civ)
    {
        // 文明50以上なら「都市」
        if (civ >= 50)
        {
            ChangeSprite(stage3_Civil);
        }
        // 自然30以上なら「森」
        else if (nature >= 30)
        {
            ChangeSprite(stage2_Nature);
        }
        // 水20以上なら「海」
        else if (water >= 20)
        {
            ChangeSprite(stage1_Water);
        }
        // それ以外（初期状態）は「荒野」
        else
        {
            ChangeSprite(stage0_Wasteland);
        }
    }

    // 画像を入れ替える用の関数
    void ChangeSprite(Sprite newSprite)
    {
        // 画像がセットされていなければ何もしない
        if (newSprite == null || targetImage == null) return;

        targetImage.sprite = newSprite;
    }
}
