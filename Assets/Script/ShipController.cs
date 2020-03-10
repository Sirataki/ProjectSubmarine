//===================================================================
//船のスクリプト（移動、ゴミの生成）
//銘苅朝香 MekaruAsuka
//===================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Items
{
    public GameObject Item;
    public float itemCool;
    public float itemCoolMin;
    public float itemCoolMax;
    public float coolDelta;
}

public class ShipController : MonoBehaviour
{
    //+-+-+-+-+-+-+-+-+-+変数+-+-+-+-+-+-+-+-+-+//

    ////ゴミのオブジェクト
    //public GameObject trashBoots, MicroWave, IceBox;
    ////トラップネットのオブジェクト
    //public GameObject trapNet;

    private GameObject NormCanvas;
    //GameManagerを取得
    private GameObject GameManager;
    GameManager gamemanager;


    public List<Items> items = new List<Items>();

    //船の向き
    private float fShipDirection;
    //コントローラの方向
    private float fController;
    //船の今の速度
    private int iNowSpeed;
    //船の前回の方向
    float fShipOldDirection;

    //自分の位置
    Transform myPosi;

    //NormTrashを取得
    NormTrash normCanvas;

    //+-+-+-+-+-+-+-+-+-+変数シンボル+-+-+-+-+-+-+-+-+-+//

    //船と生成されるゴミの距離
    public Vector3 createTrashDistance;
    //船と生成されるトラップネットの距離
    public Vector3 createTrapNetDistance;

    //船のエンジンブレーキ
    public int iShipEngineBrake = 1;
    //船のブレーキ
    public int iShipBrake = 6;
    //船の加速度
    public int iShipAcceleration = 10;
    //船の最大速度
    public int iShipMaxSpeed = 100;
    //船の最大位置
    public float fShipMaxPosi = 9;
    //船が最大位置を超えたら指定の座標に戻す数字
    public float fShipBackPosi = 9;
    //船の向きの角度
    public float fShipDirectionLeft = 180;//左移動
    public float fShipDirectionRight = 10;//右移動
    //船の回転の早さ
    public float fShipRotaSpeed = 20;//180 / fShipRotaSpeedで割って余りが出ないようにしてください

    //魚雷が当たったときに作られる冷蔵庫の数
    public int iFlyIceBoxMax = 3;
    //魚雷が当たったときに冷蔵庫が飛び出す速さ
    public float fFlyIceBoxSpeed = 100;
    //魚雷が当たったときに冷蔵庫が飛び出すX
    public float fFlyIceBoxX = 1.3f;
    //魚雷が当たったときに冷蔵庫が飛び出すY
    public float fFlyIceBoxY = 5.5f;

    //あたった時に出るパーティクルシステム
    public ShipParticle sparticle;

    //+-+-+-+-+-+-+-+-+-+初期化+-+-+-+-+-+-+-+-+-+//
    void Start()
    {
        //自分の座標を当てはめ初期化
        myPosi = this.transform;
        //とりあえず0に初期化
        fShipDirection = 0;

        //NormTrashを取得
        NormCanvas = GameObject.Find("NormTrashCanvas");
        normCanvas = NormCanvas.GetComponent<NormTrash>();

        //GameManagerを取得
        GameManager = GameObject.Find("GameManager");
        gamemanager = GameManager.GetComponent<GameManager>();
    }

    //+-+-+-+-+-+-+-+-+-+アップデート+-+-+-+-+-+-+-+-+-+//
    void Update()
    {
        for (int i = 0; i < items.Count; i++)
        {
            items[i].coolDelta += Time.deltaTime;
            items[i].itemCool = items[i].itemCoolMin + (items[i].itemCoolMax - items[i].itemCoolMin) / gamemanager.gameStartTime * gamemanager.gameTime;
        }

        //↓↓デバッグ用の処理↓↓
        //if (Input.GetKey(KeyCode.LeftArrow)) getControllShip(-1.0f);//左入力で船を左へ移動させる(horizontalを想定)
        //else if (Input.GetKey(KeyCode.RightArrow)) getControllShip(1.0f);//右入力で船を右へ移動させる(horizontalを想定)
        //else getControllShip(0.0f);

        //if (Input.GetKeyUp(KeyCode.Z)) createTrashBoots();// " Z " キーで長靴を生成
        //if (Input.GetKeyUp(KeyCode.X)) createTrashMicroWave();// " X " キーで電子レンジを生成
        //if (Input.GetKeyUp(KeyCode.C)) createTrashIceBox();// " C " キーで冷蔵庫を生成
        //if (Input.GetKeyUp(KeyCode.V)) createTrapNet();// " V " キーでトラップネットを生成
        //↑↑デバッグ用の処理↑↑

        //船の動きと回転
        myPosi = moveShip(myPosi, fController, fShipDirection);
        //ノルマの数字を自分の位置と同期させる
        normCanvas.transform.position = myPosi.position + normCanvas.myPosition;

        //範囲外を超えたらスピードを止め、ポジションを少し戻す
        if (myPosi.position.x >= fShipMaxPosi) { myPosi.position = new Vector3(fShipBackPosi, myPosi.position.y, myPosi.position.z); iNowSpeed = 0; }
        if (myPosi.position.x <= -fShipMaxPosi) { myPosi.position = new Vector3(-fShipBackPosi, myPosi.position.y, myPosi.position.z); iNowSpeed = 0; }

        //これまでの処理を船に反映させる
        this.transform.position = myPosi.position;
        this.transform.localEulerAngles = myPosi.localEulerAngles;

    }

    //+-+-+-+-+-+-+-+-+-+船の動き+-+-+-+-+-+-+-+-+-+//
    private Transform moveShip(Transform getMoveShip, float fAccel, float fAngle)
    {
        //数字をゲーム内の数字に合わせるための数字
        const int fitNum2Game = 1000;

        if (fAccel == 0)//エンジンブレーキ
        {
            //右に移動中ならスピードを落とす
            if (iNowSpeed > 0.0f) iNowSpeed -= iShipEngineBrake;
            //左に移動中ならスピードを落とす
            if (iNowSpeed < 0.0f) iNowSpeed += iShipEngineBrake;
        }
        else if (fAccel < 0)//左移動(Horizonが-1なら)
        {
            //船が進む方向に向いていなければ回転させる
            if (getMoveShip.localEulerAngles.y < fAngle)
            {
                //船を回転
                getMoveShip.localEulerAngles += new Vector3(0, fShipRotaSpeed, 0);
            }
            else
            {
                //右移動中ならスピードを落とす
                if (iNowSpeed > 0) iNowSpeed += (int)(fAccel * iShipBrake);
                //左移動中、もしくは停止中かつ最大スピード以上ならスピードを上げる
                if (iNowSpeed <= 0 && iNowSpeed >= -iShipMaxSpeed) iNowSpeed += (int)(fAccel * iShipAcceleration);
            }
        }
        else if (fAccel > 0)//右移動(Horizonが1なら)
        {
            //船が進む方向に向いていなければ回転させる
            if (getMoveShip.localEulerAngles.y > fAngle)
            {
                //船を回転
                getMoveShip.localEulerAngles += new Vector3(0, -fShipRotaSpeed, 0);
            }
            else
            {
                //左移動中ならスピードを落とす
                if (iNowSpeed < 0) iNowSpeed += (int)(fAccel * iShipBrake);
                //右移動中、もしくは停止中かつ最大スピード以上ならスピードを上げる
                if (iNowSpeed >= 0 && iNowSpeed <= iShipMaxSpeed) iNowSpeed += (int)(fAccel * iShipAcceleration);
            }
        }

        getMoveShip.position += new Vector3(iNowSpeed, 0, 0) / fitNum2Game;
        

        return getMoveShip;
    }

    //+-+-+-+-+-+-+-+-+-+船の操作の取得+-+-+-+-+-+-+-+-+-+//
    public void getControllShip(float fMoveShip)
    {
        //コントローラーの向きを自分のスクリプト内に保存
        fController = fMoveShip;

        //船の向きを自分のスクリプト内に保存
        if (fMoveShip != 0) {
            fShipOldDirection = fMoveShip;
            //SE
            //soundManager.Instance.PlaySound(3, true);
        }
        fShipDirection = (fShipOldDirection > 0) ? fShipDirectionRight : fShipDirectionLeft;
    }

    //+-+-+-+-+-+-+-+-+-+アイテムを生成する座標を決める+-+-+-+-+-+-+-+-+-+//
    private Vector3 createItemPosi(Vector3 getCreateItemPosi)
    {
        Vector3 returnPosi;

        returnPosi.x = myPosi.position.x + changeSign(getCreateItemPosi.x);
        returnPosi.y = myPosi.position.y + getCreateItemPosi.y;
        returnPosi.z = myPosi.position.z + getCreateItemPosi.z;

        return returnPosi;
    }

    //+-+-+-+-+-+-+-+-+-+船の向きで符号を変える+-+-+-+-+-+-+-+-+-+//
    private float changeSign(float fchangeSignNum)
    {
        if (fShipDirection == fShipDirectionRight) fchangeSignNum = -fchangeSignNum;
        return fchangeSignNum;
    }

    //+-+-+-+-+-+-+-+-+-+長靴のゴミを生成+-+-+-+-+-+-+-+-+-+//
    public void createTrashBoots()
    {
        if (items[0].itemCool > items[0].coolDelta)
        {
            return;
        }
        //SE
        soundManager.Instance.PlaySound(7,false);
        Instantiate(items[0].Item, createItemPosi(createTrashDistance), Quaternion.identity);
        items[0].coolDelta = 0;
    }

    //+-+-+-+-+-+-+-+-+-+電子レンジのゴミを生成+-+-+-+-+-+-+-+-+-+//
    public void createTrashMicroWave()
    {
        if (items[1].itemCool > items[1].coolDelta)
        {
            return;
        }
        //SE
        soundManager.Instance.PlaySound(8,false);
        Instantiate(items[1].Item, createItemPosi(createTrashDistance), Quaternion.identity);
        items[1].coolDelta = 0;

    }

    //+-+-+-+-+-+-+-+-+-+冷蔵庫のゴミを生成+-+-+-+-+-+-+-+-+-+//
    public void createTrashIceBox()
    {
        if (items[2].itemCool > items[2].coolDelta)
        {
            return;
        }
        //SE
        soundManager.Instance.PlaySound(9,false);
        Instantiate(items[2].Item, createItemPosi(createTrashDistance), Quaternion.identity);
        items[2].coolDelta = 0;
    }

    //+-+-+-+-+-+-+-+-+-+トラップネットを生成+-+-+-+-+-+-+-+-+-+//
    public void createTrapNet()
    {
        if (items[3].itemCool > items[3].coolDelta)
        {
            return;
        }
        //SE
        soundManager.Instance.PlaySound(8,false);
        Instantiate(items[3].Item, createItemPosi(createTrapNetDistance), Quaternion.identity);
        items[3].coolDelta = 0;
    }

    //+-+-+-+-+-+-+-+-+-+当たり判定+-+-+-+-+-+-+-+-+-+//
    void OnTriggerEnter(Collider other)
    {
        //魚雷に当たった処理
        if (other.gameObject.tag == "Torpedo")
        {
            //SE
            soundManager.Instance.PlaySound(13, false);
            normCanvas.iNormNum += 10;

            sparticle.PlayParticle(other.transform.position);

            for (int i = 0; i < iFlyIceBoxMax; i++)
            {
                //魚雷が当たれば飛ぶオブジェクトを指定
                GameObject throwIceBox = Instantiate(items[1].Item) as GameObject;
                //冷蔵庫の発生位置
                throwIceBox.transform.position = myPosi.position;
                //冷蔵庫の行き先
                Vector3 goToPoint = new Vector3(Random.Range(-fFlyIceBoxX, fFlyIceBoxX), fFlyIceBoxY, 0) - throwIceBox.transform.position;
                //冷蔵庫を実際に飛ばす
                throwIceBox.GetComponent<Rigidbody>().AddForce(goToPoint * fFlyIceBoxSpeed);
            }
            Destroy(other.gameObject);
        }
    }
}
