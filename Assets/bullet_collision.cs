using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_collision : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
//	void OnTriggerEnter(Collider other){
//		if (other.gameObject.CompareTag ("bullet")) {
//			Destroy (other.gameObject);
//		}
//	}
	void OnCollisionEnter(Collision col){
		if(col.gameObject.CompareTag("bullet")){
			Destroy (col.gameObject);
		}
	}
}
