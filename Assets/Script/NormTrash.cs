//===================================================================
//ノルマのスクリプト（ノルマの数字）
//銘苅朝香 MekaruAsuka
//===================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NormTrash : MonoBehaviour
{
    //+-+-+-+-+-+-+-+-+-+変数シンボル+-+-+-+-+-+-+-+-+-+//

    //カウントストップする数
    private const int iCountStop = 99;
    //ノルマの数
    public int iNormNum;
    //自分の位置の調整
    public Vector3 myPosition;

    public TrashCount receivePoint;

    //+-+-+-+-+-+-+-+-+-+変数+-+-+-+-+-+-+-+-+-+//

    //作ったゴミの数
    public int iOldCreateTrash, iNowCreateTrash;

    //画像を変えるときに使う数字
    int tmpCreateTrash, tmpNorm;

    public Text normText; 

    //+-+-+-+-+-+-+-+-+-+初期化+-+-+-+-+-+-+-+-+-+//
    void Start()
    {
        normText.text = iNowCreateTrash.ToString("D2") + "/" + iNormNum.ToString("D2");

        receivePoint = GameObject.Find("Sea Floor").GetComponent<TrashCount>();
    }

    //+-+-+-+-+-+-+-+-+-+アップデート+-+-+-+-+-+-+-+-+-+//
    void Update()
    {
        normText.text = receivePoint.achieveCount.ToString("D2") + "/" + iNormNum.ToString("D2");

        //カンスト時の処理
        //if (iNowCreateTrash > iCountStop) return;
        if (receivePoint.shipPoints > iCountStop) return;

    }

}

