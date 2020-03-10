using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamepadInput;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleController : MonoBehaviour {

    private bool is1pFlag = false, is2pFlag = false, isEndHoeTo = false;

    [SerializeField]
    private GameObject titleLogo, howTo, check1p, check2p;

    public Image howtoIcon;

    private Text readyState1p, readyState2p;

    private TextColorChange textColorChange1, textColorChange2;

    public Sprite[] howtoSprite = new Sprite[2];

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

    private void Start()
    {
        readyState1p = check1p.GetComponent<Text>();
        readyState2p = check2p.GetComponent<Text>();

        textColorChange1 = readyState1p.GetComponent<TextColorChange>();
        textColorChange2 = readyState2p.GetComponent<TextColorChange>();

        soundManager.Instance.StopBgm();
        soundManager.Instance.ChangeBgm(0);
        titleLogo = GameObject.Find("TitleLogo");
        readyState1p = check1p.GetComponent<Text>();
        readyState2p = check2p.GetComponent<Text>();
    }

    // Update is called once per frame
    private void Update ()
    {

        //// 1P, 2Pのフラグが立っている間のみチェックマークを表示
        //check1p.SetActive(is1pFlag);
        //check2p.SetActive(is2pFlag);

        if (!Is1pFlag)
        {
            textColorChange1.TextFade();
        }

        if (!Is2pFlag)
        {
            textColorChange2.TextFade();
        }

        if (!Is1pFlag && !Is2pFlag) return;

        if (Is1pFlag)
        {
            if(readyState1p.text != "1P Ready")
            {
                readyState1p.text = "1P Ready";
                readyState1p.color = new Color(readyState1p.color.r, readyState1p.color.g, readyState1p.color.b, 1f);
                soundManager.Instance.PlaySound(0, false);
            }
        }

        if (Is2pFlag)
        {
            if(readyState2p.text != "2P Ready")
            {
                readyState2p.text = "2P Ready";
                readyState2p.color = new Color(readyState2p.color.r, readyState2p.color.g, readyState2p.color.b, 1f);
                soundManager.Instance.PlaySound(0, false);
            }
        }

        if (Is1pFlag && Is2pFlag)
        {
            if (!howTo.activeSelf)
            {
                Is1pFlag = false;
                Is2pFlag = false;

                titleLogo.SetActive(false);
                howTo.SetActive(true);
                howtoIcon.sprite = howtoSprite[0];

                Invoke("HowToVisible", 0.2f);
            }
            else if (howTo.activeSelf && !isEndHoeTo)
            {
                Is1pFlag = false;
                Is2pFlag = false;

                isEndHoeTo = true;
                howtoIcon.sprite = howtoSprite[1];

            }
            else if (howTo.activeSelf && isEndHoeTo)
            {
                Invoke("SceneChange", 0.5f);
            }
        }

    }

    private void HowToVisible()
    {
        readyState1p.text = "Push any key";
        readyState2p.text = "Push any key";
    }

    private void SceneChange()
    {
        SceneManager.LoadScene("gamemain");
    }
    
}
