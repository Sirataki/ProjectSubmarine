using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InScoreText : MonoBehaviour {
    
    public void PointSet(int p){
        GetComponent<TextMesh>().text = p.ToString();
    }

	public void DestroyObj(){
		Destroy(this.gameObject);
	}
}
