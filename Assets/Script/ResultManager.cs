using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultManager : MonoBehaviour
{
    [SerializeField]
    private GameObject ready1p, ready2p;

    private Text ready1pText, ready2pText;

    private TextColorChange tColorChange1p, tColorChange2p;

    public bool ready1pFlag = false, ready2pFlag = false;

    //短いドラムロールの秒数
    private const int ShortDrumRollCnt = 60;

    //長いドラムロールの秒数
    private const int LongDrumRollCnt = 120;

    //点差の上限
    private const int diffPointMax = 20;

    //船のポイント
    private int shipPoint;

    //船のポイントのテキスト
    public UnityEngine.UI.Text shipPointText;

    //潜水艦のポイントのテキスト
    public UnityEngine.UI.Text submarinePointText;

    //リサルトの画像
    public GameObject resultImgPanel;
    public Sprite[] sprite;
    UnityEngine.UI.Image resultImage;

    //フレームカウント
    private int frameCnt;

    //ドラムロールカウント
    private int DrumRollCnt;

    private bool is1pFlag = false, is2pFlag = false;

    private int drumNum;

    // 1P及び2Pの準備の状態を読み書きするプロパティ
    public bool Is1pFlag
    {
        get { return is1pFlag; }
        set { is1pFlag = value; }
    }
    public bool Is2pFlag
    {
        get { return is2pFlag; }
        set { is2pFlag = value; }
    }

    public enum PHASE
    {
        PHASE_DRUMROLL,
        PHASE_SHIPNORM,
        PHASE_JUDGE,
        PHASE_RETURNTITLE
    }

    private PHASE resultPhase;

    bool drumEnd;

    private bool backFlag = false;
    public bool BackFlagProp
    {
        get { return backFlag; }
        set { backFlag = value; }
    }

    void Start()
    {
        //BGM
        soundManager.Instance.StopBgm();

        ready1pText = ready1p.GetComponent<Text>();
        ready2pText = ready2p.GetComponent<Text>();

        tColorChange1p = ready1pText.GetComponent<TextColorChange>();
        tColorChange2p = ready2pText.GetComponent<TextColorChange>();

        frameCnt = 0;

        shipPoint = GameManager.instance.GetShipPoint();
        
        int submarinePoint = GameManager.instance.GetSubmarinePoint();

        int diffPoint = shipPoint - submarinePoint;

        //ドラムロールの秒数設定
        if (System.Math.Abs(diffPoint) <= diffPointMax)
        {
            DrumRollCnt = LongDrumRollCnt;
            drumNum = 16;
        }
        else
        {
            DrumRollCnt = ShortDrumRollCnt;
            drumNum = 15;
        }

        //SE
        soundManager.Instance.PlaySound(drumNum, false);

        resultImage = resultImgPanel.gameObject.GetComponent<UnityEngine.UI.Image>();

        resultPhase = PHASE.PHASE_DRUMROLL;;
    }

    void Update()
    {
        switch (resultPhase)
        {
            case PHASE.PHASE_SHIPNORM: ShipNorm(); break;
            case PHASE.PHASE_DRUMROLL: DrumRoll(); break;
            case PHASE.PHASE_JUDGE: Judge(); break;
            case PHASE.PHASE_RETURNTITLE: ReturnToTitle(); break;
        }

        if (BackFlagProp)
        {
            if (!ready1pFlag)
            {
                tColorChange1p.TextFade();
            }
            if (!ready2pFlag)
            {
                tColorChange2p.TextFade();
            }
            
            if (ready1pFlag)
            {
                if (ready1pText.text == "1P Ready")
                {
                    return;
                }
                ready1pText.text = "1P Ready";
                ready1pText.color = new Color(ready1pText.color.r, ready1pText.color.g, ready1pText.color.b, 1.0f);
            }
            if (ready2pFlag)
            {
                if (ready2pText.text == "2P Ready")
                {
                    return;
                }
                ready2pText.text = "2P Ready";
                ready2pText.color = new Color(ready2pText.color.r, ready2pText.color.g, ready2pText.color.b, 1.0f);
            }

        }
        
    }

    private void DrumRoll()
    {
        int randPoint = Random.Range(0, 1000);

        shipPointText.text = randPoint.ToString();

        randPoint = Random.Range(0, 1000);

        submarinePointText.text = randPoint.ToString();

        if (++frameCnt > DrumRollCnt)
        {
            shipPointText.text = GameManager.instance.GetShipPoint().ToString();

            submarinePointText.text = GameManager.instance.GetSubmarinePoint().ToString();

            resultPhase = PHASE.PHASE_SHIPNORM;
        }
    }

    private void ShipNorm()
    {
        Invoke("ShipNormMinus", 1.0f);
    }

    private void ShipNormMinus()
    {
        if (!drumEnd)
        {
            drumEnd = true;

            //SE
            soundManager.Instance.PlaySound(17, false);
        }
        --shipPoint;

        shipPointText.text = shipPoint.ToString();

        int trueShipPoint = GameManager.instance.Get1TrueShipPoint();

        if (shipPoint <= trueShipPoint)
        {
            shipPoint = trueShipPoint;

            shipPointText.text = shipPoint.ToString();

            resultPhase = PHASE.PHASE_JUDGE;
        }
    }

    private void Judge()
    {
        if (GameManager.instance.GetWinner() == GameManager.WINNER.WINNER_SHIP)
        {
            resultImage.sprite = sprite[0];
        }
        else if (GameManager.instance.GetWinner() == GameManager.WINNER.WINNER_SUBMARINE)
        {
            resultImage.sprite = sprite[1];
        }
        else
        {
            resultImage.sprite = sprite[2];
        }

        BackFlagProp = true;

        ready1p.SetActive(true);
        ready2p.SetActive(true);

        Invoke("ResultBgm", 1f);
    }
    private void ResultBgm()
    {
        soundManager.Instance.ChangeBgm(2);
    }
    public bool ReturnToTitle()
    {
        return BackFlagProp;
    }
}