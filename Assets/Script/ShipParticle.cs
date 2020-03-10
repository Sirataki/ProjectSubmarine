using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipParticle : MonoBehaviour {
    
	private ParticleSystem sparkPt;

	private void Start()
	{
		sparkPt = GetComponent<ParticleSystem>();
	}
	public void PlayParticle (Vector3 pos) 
	{
		this.transform.position = pos;

		sparkPt.Play();
	}
}
