//===================================================================
//ゴミ関連のスクリプト（主に重力）
//銘苅朝香 MekaruAsuka
//===================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trashes : MonoBehaviour
{

    //+-+-+-+-+-+-+-+-+-+変数+-+-+-+-+-+-+-+-+-+//

    //このvectorで重力を決める(Unityで設定)
    public Vector3 localGravity;
    private Rigidbody rb;
    public int pointsSh;
    public int pointsSu;

    //+-+-+-+-+-+-+-+-+-+初期化+-+-+-+-+-+-+-+-+-+//
    void Start()
    {
        //重力の初期化
        rb = this.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    //+-+-+-+-+-+-+-+-+-+アップデート+-+-+-+-+-+-+-+-+-+//
    void FixedUpdate()
    {
        setLocalGravity();
    }


    //+-+-+-+-+-+-+-+-+-+重力設定+-+-+-+-+-+-+-+-+-+//
    void setLocalGravity()
    {
        rb.AddForce(localGravity, ForceMode.Acceleration);
    }

    //+-+-+-+-+-+-+-+-+-+当たり判定+-+-+-+-+-+-+-+-+-+//
    void OnCollisionEnter(Collision collision)
    {
        Destroy(this.gameObject);
    }

}

