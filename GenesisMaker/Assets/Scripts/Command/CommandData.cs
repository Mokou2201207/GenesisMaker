using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// コマンド（選択の部分）のステータス
/// </summary>
[CreateAssetMenu(fileName = "NewCommand", menuName = "Genesis/CommandData")]
public class CommandData : ScriptableObject
{
    [Header("コマンドの名前")]
    public string commandName;
    [Header("コマンドの説明")]
    [TextArea] public string descripyion;

    [Header("水分（＋で潤う、－で乾燥）")]
    public int waterChange;
    [Header("気温（＋で温暖、－で寒冷）")]
    public int tempChange;
    [Header("自然（＋で緑化、－で荒廃）")]
    public int natureChange;
    [Header("文明（＋で発展、－で衰退）")]
    public int civChange;
}
