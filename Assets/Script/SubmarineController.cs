using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineController : MonoBehaviour
{

    public float speed = 0.0f;//左右移動スピード
    [HideInInspector]
    public int count = 0;//現在の所持数
    public int countMax = 5;//限界の所持数
    [HideInInspector]
    public int point = 0;//潜水艦の暫定得点
    [HideInInspector]
    public int enterPoint = 0;//潜水艦の確定得点

    private Transform myPosi;
    private float fControllerX;
    private float fControllerY;
    private float fSubmarineDirection;
    private float iNowSpeed;
    private float iNowSpeedY;

    public float iSubmarineEngineBrake;
    public float iSubmarineBrake;
    public float iSubmarineAcceleration;
    public int iSubmarineMaxSpeed;
    private float fSubmarineOldDirection;

    public float fSubmarineDirectionRight = 90;
    public float fSubmarineDirectionLeft = -90;

    //船の回転の早さ
    public float fShipRotaSpeed = 20;//180 / fShipRotaSpeedで割って余りが出ないようにしてください

    private bool MissileFlg = false;//trueで発射可能
    private Vector3 position;

    private GameObject submarineModel;
    private bool NetFlg;//trueで停止
    private float frameCnt;//待ち時間
    public float frameCntMax;//停止時間

    [HideInInspector]
    public GameObject saveCollObject;

    [SerializeField]
    private GameObject missile;

    [SerializeField]
    private GameObject shipTarget, missileIcon;


    // Use this for initialization
    void Start()
    {
        myPosi = this.transform;
        submarineModel = GameObject.Find("sabmarin");
        //とりあえず0に初期化
        fSubmarineDirection = 0;

        frameCnt = 0;//初期化
        NetFlg = false;//操作可能
    }

    // Update is called once per frame
    void Update()
    {
        //潜水艦の動き
        if (!NetFlg)
        {
            myPosi = moveSubmarineX(myPosi, fControllerX, fSubmarineDirection);

            myPosi.position += moveSubmarineY(new Vector3(0, 0, 0), fControllerY);
        }
        else
        {
            frameCnt += Time.deltaTime;
            if (frameCnt > frameCntMax)
            {
                frameCnt = 0;
                NetFlg = false;
                Destroy(saveCollObject);
            }
        }

        ////画面外へ行かない処理
        if (myPosi.position.x >= 8.0f)
        {
            myPosi.position = new Vector3(8.0f, myPosi.position.y, myPosi.position.z);
        }
        if (myPosi.position.y >= 4.0f)
        {
            myPosi.position = new Vector3(myPosi.position.x, 4.0f, myPosi.position.z);
        }
        if (myPosi.position.x <= -7.8f)
        {
            myPosi.position = new Vector3(-7.8f, myPosi.position.y, myPosi.position.z);
        }
        if (myPosi.position.y <= -3.6f)
        {
            myPosi.position = new Vector3(myPosi.position.x, -3.6f, myPosi.position.z);
        }

        //これまでの処理を潜水艦に反映させる
        //submarineModel.transform.position = myPosi.position;
        //submarineModel.transform.eulerAngles = myPosi.eulerAngles;
    }

    //+-+-+-+-+-+-+-+-+-+潜水艦の動き+-+-+-+-+-+-+-+-+-+//
    private Transform moveSubmarineX(Transform getMoveShip, float fAccel, float fAngle)
    {
        //数字をゲーム内の数字に合わせるための数字＆割り算
        const int fitNum2Game = 1000;

        if (fAccel == 0)//エンジンブレーキ
        {
            //右に移動中ならスピードを落とす
            if (iNowSpeed > 0.0f) iNowSpeed -= iSubmarineEngineBrake;
            //左に移動中ならスピードを落とす
            if (iNowSpeed < 0.0f) iNowSpeed += iSubmarineEngineBrake;
        }
        else if (fAccel < 0)//左移動(Horizonが-1なら)
        {
            //船が進む方向に向いていなければ回転させる  //270
            if (submarineModel.transform.eulerAngles.y <= fAngle)
            {
                //船を回転
                submarineModel.transform.eulerAngles += new Vector3(0, fShipRotaSpeed, 0);
            }
            else
            {
                //右移動中ならスピードを落とす
                if (iNowSpeed > 0) iNowSpeed += (int)(fAccel * iSubmarineBrake);
                //左移動中、もしくは停止中かつ最大スピード以上ならスピードを上げる
                if (iNowSpeed <= 0 && iNowSpeed >= -iSubmarineMaxSpeed) iNowSpeed += (int)(fAccel * iSubmarineAcceleration);
            }
        }
        else if (fAccel > 0)//右移動(Horizonが1なら)
        {
            //船が進む方向に向いていなければ回転させる  //90
            if (submarineModel.transform.eulerAngles.y >= fAngle)
            {
                //船を回転
                submarineModel.transform.eulerAngles += new Vector3(0, -fShipRotaSpeed, 0);
            }
            else
            {
                //左移動中ならスピードを落とす
                if (iNowSpeed < 0) iNowSpeed += (int)(fAccel * iSubmarineBrake);
                //右移動中、もしくは停止中かつ最大スピード以上ならスピードを上げる
                if (iNowSpeed >= 0 && iNowSpeed <= iSubmarineMaxSpeed) iNowSpeed += (int)(fAccel * iSubmarineAcceleration);
            }
        }

        getMoveShip.position += new Vector3(iNowSpeed, 0, 0) / fitNum2Game;

        return getMoveShip;
    }

    private Vector3 moveSubmarineY(Vector3 getMoveSubmarine, float fAccel)
    {
        if (fAccel == 0)//エンジンブレーキ
        {
            //上に移動中ならスピードを落とす
            if (iNowSpeedY > 0.0f) iNowSpeedY -= iSubmarineEngineBrake;
            //下に移動中ならスピードを落とす
            if (iNowSpeedY < 0.0f) iNowSpeedY += iSubmarineEngineBrake;
        }
        else if (fAccel < 0)//左移動(Horizonが-1なら)
        {
            //上移動中ならスピードを落とす
            if (iNowSpeedY > 0) iNowSpeedY += (int)(fAccel * iSubmarineBrake);
            //下移動中、もしくは停止中かつ最大スピード以上ならスピードを上げる
            if (iNowSpeedY <= 0 && iNowSpeedY >= -iSubmarineMaxSpeed) iNowSpeedY += (int)(fAccel * iSubmarineAcceleration);
        }
        else if (fAccel > 0)//右移動(Horizonが1なら)
        {
            //下移動中ならスピードを落とす
            if (iNowSpeedY < 0) iNowSpeedY += (int)(fAccel * iSubmarineBrake);
            //上移動中、もしくは停止中かつ最大スピード以上ならスピードを上げる
            if (iNowSpeedY >= 0 && iNowSpeedY <= iSubmarineMaxSpeed) iNowSpeedY += (int)(fAccel * iSubmarineAcceleration);
        }
        getMoveSubmarine.y = iNowSpeedY * speed;

        const int fitNum2Game = 1000;
        return getMoveSubmarine / fitNum2Game;
    }


    //+-+-+-+-+-+-+-+-+-+潜水艦の操作の取得+-+-+-+-+-+-+-+-+-+//
    public void getControllSubmarineX(float fMoveSubmarine)
    {
        //コントローラーの向きを自分のスクリプト内に保存
        fControllerX = fMoveSubmarine;


        //潜水艦の向きを自分のスクリプト内に保存
        if (fMoveSubmarine != 0) fSubmarineOldDirection = fMoveSubmarine;
        fSubmarineDirection = (fSubmarineOldDirection > 0) ? fSubmarineDirectionRight : fSubmarineDirectionLeft;
    }

    //+-+-+-+-+-+-+-+-+-+潜水艦の操作の取得+-+-+-+-+-+-+-+-+-+//
    public void getControllSubmarineY(float fMoveSubmarine)
    {
        //コントローラーの向きを自分のスクリプト内に保存
        fControllerY = fMoveSubmarine;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Plant")//回収に当たったら所持をゼロに回収した分をポイントに
        {
            enterPoint += point;
            point = 0;
            count = 0;
            //SE
            soundManager.Instance.PlaySound(5, false);
        }

        if (other.tag == "Net")//ネットの当たり判定
        {
            NetFlg = true;
            other.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            saveCollObject = other.gameObject;//いちじほぞん
            other.gameObject.transform.position = new Vector2(this.transform.position.x, other.gameObject.transform.position.y);

            //SE
            soundManager.Instance.PlaySound(14, false);
        }

        if (count == countMax)
        {
            return;
        }

        if (other.tag == "Item")//アイテムの当たり判定
        {
            missileIcon.SetActive(true);
            MissileFlg = true;
            //SE
            soundManager.Instance.PlaySound(11, false);

            Destroy(other.gameObject);
        }

        if (other.tag == "Trash")//仮のごみの当たり判定
        {
            if (count <= countMax)
            {
                //SE
                soundManager.Instance.PlaySound(10, false);
                point += other.gameObject.GetComponent<Trashes>().pointsSu;
                count++;
            }
            Destroy(other.gameObject);
        }
    }

    public void FireTorpedo()
    {
        if (MissileFlg == true)
        {
            //SE
            soundManager.Instance.PlaySound(13, false);
            missileIcon.SetActive(false);

            // プレハブからインスタンスを生成
            GameObject missileClone =　Instantiate(missile, transform.position, Quaternion.identity);
            var aim = shipTarget.transform.position - this.transform.position;
            var look = Quaternion.LookRotation(aim, Vector3.forward);
            missileClone.transform.rotation = look;
            MissileFlg = false;
        }
    }
}
