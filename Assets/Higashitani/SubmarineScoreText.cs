using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubmarineScoreText : MonoBehaviour {

	GameObject inScorePointText;

	void OnTriggerEnter(Collider other)
	{
		if(other.name == "Submarine")
		{
			int p = other.GetComponent<SubmarineController>().point;
			GameObject clone = Instantiate(inScorePointText, transform.position, Quaternion.identity);
			clone.GetComponent<InScoreText>().PointSet(p);
		}
	}
}
