using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //readyの出てる秒数
    private const int readyCnt = 60;
    //goの出てる秒数
    private const int goCnt = 60;
    //finishの出てる秒数
    private const int finishCnt = 60;
    //ミサイルの生成確率
    private const float missileCreatePro = 0.1f;

    //シングルトンにする
    static public GameManager instance;

    [SerializeField]
    private GameObject ship, submarine;

    [HideInInspector]
    public ShipController shipController;

    [HideInInspector]
    public SubmarineController submarineController;

    [HideInInspector]
    public TrashCount trashCount;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //ゲームスタート時のテキスト
    public GameObject readyText;

    public GameObject goText;

    //ゲーム終了時のテキスト
    public GameObject finishText;

    //制限時間のテキスト
    public UnityEngine.UI.Text gameTimeText;

    //制限時間
    public float gameTime;
    //制限時間
    public float gameStartTime;

    //ゲージを消す時間
    public float gaugeDeleteTime;

    //ミサイルの最大出現個数
    public int missileNumMax;

    //ミサイルの生成時間の最大値
    private int createMissileTimeMax;

    //ミサイルプレハブ
    public GameObject missilePrefab;

    //ミサイルが生成されたかどうか
    private bool isCreateMissile;

    //フレームカウント
    private int frameCnt;

    //潜水艦の得点
    [HideInInspector]
    public static int submarinePoint;

    //船の得点
    [HideInInspector]
    public static int shipPoint;

    //実際の船の得点
    [HideInInspector]
    public static int trueShipPoint;

    [SerializeField]
    private GameObject[] torpedoIcons = new GameObject[2];

    public enum WINNER
    {
        WINNER_NOTHING,
        WINNER_SUBMARINE,
        WINNER_SHIP
    }

    [HideInInspector]
    public static WINNER winner;

    private enum PHASE
    {
        PHASE_START,
        PHASE_MAINGAME,
        PHASE_FINISH
    }

    private PHASE gamePhase;

    //勝利者の取得
    public WINNER GetWinner() { return winner; }

    //潜水艦のポイント取得
    public int GetSubmarinePoint() { return submarinePoint; }

    //船のポイント取得
    public int GetShipPoint() { return shipPoint; }

    //実際の船のポイント取得
    public int Get1TrueShipPoint() { return trueShipPoint; }

    void Start()
    {
        //BGM
        soundManager.Instance.StopBgm();
        soundManager.Instance.ChangeBgm(1);

        goText.SetActive(false);
        finishText.SetActive(false);

        submarinePoint = 0;

        shipPoint = 0;

        frameCnt = 0;

        gameTime = gameStartTime;

        gameTimeText.color = new Color(255.0f, 255.0f, 255.0f, 0.0f);

        //ミサイルの生成時間の最大値を決める
        createMissileTimeMax = (int)gameTime * 60 / missileNumMax;

        isCreateMissile = false;

        winner = WINNER.WINNER_NOTHING;

        gamePhase = PHASE.PHASE_START;

        shipController = ship.GetComponent<ShipController>();
        trashCount = GameObject.Find("Sea Floor").GetComponent<TrashCount>();
        submarineController = submarine.GetComponent<SubmarineController>();
    }

    void Update()
    {
        switch (gamePhase)
        {
            case PHASE.PHASE_START: GameStart(); break;
            case PHASE.PHASE_MAINGAME: GameMain(); break;
            case PHASE.PHASE_FINISH: GameFinish(); break;
        };
    }

    private void GameStart()
    {
        ++frameCnt;

        if (frameCnt > readyCnt + goCnt)
        {
            goText.SetActive(false);
            gamePhase = PHASE.PHASE_MAINGAME;

            frameCnt = 0;
        }
        else if (frameCnt > readyCnt)
        {
            if(!goText.activeSelf)
            //SE
            soundManager.Instance.PlaySound(1, false);
            readyText.SetActive(false);
            goText.SetActive(true);
        }
    }

    private void GameMain()
    {
        float randNum = Random.Range(0.0f, 100.0f);
        int rNum = Random.Range(0, 2);
        
        //ミサイルの出現抽選
        if (!isCreateMissile)
        {
            if (randNum < missileCreatePro)
            {
                GameObject item = Instantiate(missilePrefab, transform.position, missilePrefab.gameObject.transform.rotation);
                item.GetComponent<ItemMissile>().RandSet(rNum);
                torpedoIcons[rNum].gameObject.SetActive(true);
                Invoke("HideIcon", 1f);

                isCreateMissile = true;
            }
        }

        //生成時間の最大値を超えた
        if (++frameCnt > createMissileTimeMax)
        {
            //ミサイルが生成されていなければ生成
            if (!isCreateMissile)
            {
                GameObject item = Instantiate(missilePrefab, transform.position, missilePrefab.gameObject.transform.rotation);
                item.GetComponent<ItemMissile>().RandSet(rNum);
                torpedoIcons[rNum].gameObject.SetActive(true);
                Invoke("HideIcon", 1f);
            }

            frameCnt = 0;

            isCreateMissile = false;
        }

        //得点の更新
        submarinePoint = submarineController.enterPoint;

        shipPoint = trashCount.shipPoints;

        //ゲージの更新
        //船ゲージの取得
        GameObject shipGauge = GameObject.Find("ShipGauge").gameObject;

        //潜水艦ゲージの取得
        GameObject submarineGauge = GameObject.Find("SubmarineGauge").gameObject;

        //全体の得点を求める
        float totalPoint = shipPoint + submarinePoint;

        //トータルポイントが0でない時にゲージを更新する
        if (totalPoint > 0)
        {
            //全体の得点から船と潜水艦の得点の割合を求める
            float shipPointRait = (float)shipPoint / totalPoint;
            float submarinePointRait = (float)submarinePoint / totalPoint;

            //割合に応じたゲージの切り出し位置にする
            //船側ゲージの更新
            shipGauge.GetComponent<RectTransform>().sizeDelta = new Vector2(50.0f, 270.0f * shipPointRait);

            //潜水艦側ゲージの更新
            submarineGauge.GetComponent<RectTransform>().sizeDelta = new Vector2(50.0f, 270.0f * submarinePointRait);
        }

        //制限時間の表示
        gameTimeText.text = gameTime.ToString("f1");

        gameTime -= Time.deltaTime;

        if (gameTime < 0)
        {
            gameTimeText.color = new Color(255.0f, 0.0f, 0.0f, 0.0f);

            gameTime = 0.0f;

            gamePhase = PHASE.PHASE_FINISH;
        }
        else if (gameTime < 10)
        {
            gameTimeText.color = new Color(255.0f, 0.0f, 0.0f, 0.5f);
        }
        else if (gameTime < gaugeDeleteTime)
        {
            shipGauge.GetComponent<Image>().color = new Color(255.0f, 0.0f, 0.0f, 0.0f);
            submarineGauge.GetComponent<Image>().color = new Color(0.0f, 0.0f, 255.0f, 0.0f);

            GameObject shipIcon = GameObject.Find("ShipIcon").gameObject;
            shipIcon.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f, 0.0f);

            GameObject submarineIcon = GameObject.Find("SubmarineIcon").gameObject;
            submarineIcon.GetComponent<Image>().color = new Color(255.0f, 255.0f, 255.0f, 0.0f);
        }
        else
        {
            gameTimeText.color = new Color(255.0f, 255.0f, 255.0f, 0.3f);
        }
    }

    private void GameFinish()
    {
        if (finishText == null || finishText.activeSelf)
        {
            return;
        }
        //SE
        soundManager.Instance.PlaySound(2, false);

        finishText.SetActive(true);

        submarinePoint = submarineController.enterPoint;

        shipPoint = trashCount.shipPoints;

        //NormTrashを取得
        GameObject normCanvas = GameObject.Find("NormTrashCanvas").gameObject;

        //船のノルマ取得
        int shipNorm = normCanvas.GetComponent<NormTrash>().iNormNum;
        //船のゴミ生成数取得
        int shipNormNum = normCanvas.GetComponent<NormTrash>().receivePoint.achieveCount;

        //ノルマの差分を求める
        int diffShipNorm = shipNormNum - shipNorm;

        if (diffShipNorm < 0)
        {
            trueShipPoint = shipPoint + diffShipNorm;
        }
        else
        {
            trueShipPoint = shipPoint;
        }

        GameJudge();

        //リザルトシーンへ
        Invoke("ToResult", 3f);
    }

    private void GameJudge()
    {
        if (submarinePoint > trueShipPoint)
        {
            winner = WINNER.WINNER_SUBMARINE;
        }
        else if (submarinePoint < trueShipPoint)
        {
            winner = WINNER.WINNER_SHIP;
        }
        else
        {
            winner = WINNER.WINNER_NOTHING;
        }
    }

    private void ToResult()
    {
        ControllerManager.Instance.scene = ControllerManager.Scene.Result;
        SceneManager.LoadScene("result");
    }

    private void HideIcon()
    {
        for (int i = 0; i < torpedoIcons.Length; i++)
        {
            torpedoIcons[i].gameObject.SetActive(false);
        }
    }
}
