using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwipeDetector : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector2 startPos;

    /// <summary>
    /// 最初に触ったところを取得
    /// </summary>
    /// <param name="eventData"></param>
    public void OnBeginDrag(PointerEventData eventData)
    {
        //指を置いたところを取得
        startPos = eventData.position;
    }

    public void OnDrag(PointerEventData eventData)
    {
       
    }

    /// <summary>
    /// 最後に離したところを取得
    /// </summary>
    /// <param name="eventData"></param>
    public void OnEndDrag(PointerEventData eventData)
    {
        //どこで
        Vector2 endPos=eventData.position;
        float diffX=endPos.x - startPos.x;

        // 50ピクセル以上スライドさせていたらスワイプと判定
        if (Mathf.Abs(diffX) > 50f)
        {
            if (diffX > 0)
            {
                // 右スワイプ（前のページへ）
                GameManager.Instance.HoldPrevPage();
            }
            else
            {
                // 左スワイプ（次のページへ）
                GameManager.Instance.HoldNextPage();
            }
        }
    }
}
