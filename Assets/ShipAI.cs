using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipAI : MonoBehaviour {

	//船の入力管理のスクリプト
	private ShipController shipController;
	//移動方向を変えるまでの時間
	public float moveRete = 0.5f;
	//時間保存
	private float timer = 0;
	//現在の方向
	private float nowAxis;

	private GameObject submarineObj;

	private List<Items> shipItemList = new List<Items>();

	private enum AIState
	{
		AI_Random,
		AI_Chase,
		AI_Reverse
	}
	private AIState aiState;
	void Start () {
		if(!ControllerManager.aiFlag){
			Destroy(this);
		}
		shipController = GetComponent<ShipController>();
		submarineObj = GameObject.Find("Submarine");
		shipItemList = shipController.items;
	}
	
	// Update is called once per frame
	void Update () 
	{
		timer += Time.deltaTime;
		//指定時間たったあと一度きり入る移動処理
		if(timer > moveRete)
		{
			switch(aiState)
			{
				case AIState.AI_Chase:
					nowAxis = AIChase();
					break;
				case AIState.AI_Random:
					nowAxis = AIRandom();
					break;
				case AIState.AI_Reverse:
					nowAxis = AIReverse();
					break;
			}
			timer = 0;
		}
		else
		{
			shipController.getControllShip(nowAxis);
		}

		//アイテムを落とす処理
		//アイテム数分回す
		for(int i = 0; i < shipItemList.Count; i++)
		{
			switch(i)
			{
				case 0:
					shipController.createTrashBoots();
					break;
				case 1:
					shipController.createTrashMicroWave();
					break;
				case 2:
					shipController.createTrashIceBox();
					break;
				case 3:
					shipController.createTrapNet();
					break;
			}
		}
	}
	private float AIRandom()
	{
		return Random.Range(-1,2);
	}

	private float AIChase()
	{
		return SubmarineDir();
	}

	private float AIReverse()
	{
		return -SubmarineDir();
	}

	private float SubmarineDir()
	{
		float x = 0;
		//左方向に潜水艦
		if(this.transform.position.x > submarineObj.transform.position.x)
		{
			x = Random.Range(0, -1);
		}
		//右方向に潜水艦
		else if(this.transform.position.x < submarineObj.transform.position.x)
		{
			x = Random.Range(0, 1);
		}
		return x;
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.tag == "Plant")
		{
			nowAxis *= -1;
		}
	}
}
