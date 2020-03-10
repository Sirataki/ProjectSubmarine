using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointContller : MonoBehaviour
{
    [SerializeField]
    private GameObject submarin, leftDust, RightDust;
    SubmarineController smc;
    string cou;

    TextMesh possessionText;

    // Use this for initialization
    void Start ()
    {
        possessionText = GetComponent<TextMesh>();
        smc = submarin.GetComponent<SubmarineController>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        //満タンだったら、赤字にしてアイコンをだす
        if(smc.count == smc.countMax)
        {
            possessionText.color = Color.red;
            leftDust.SetActive(true);
            RightDust.SetActive(true);
        }
        else
        {
            possessionText.color = Color.white;
            leftDust.SetActive(false);
            RightDust.SetActive(false);
        }

        cou = smc.count.ToString();
        Vector3 Pos = submarin.transform.position;
        transform.position = new Vector3(Pos.x + 1.0f, Pos.y - 0.5f, Pos.z + 1.0f);
        possessionText.text = cou + "/5";
    }
}
