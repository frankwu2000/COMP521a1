using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bullet_boulder : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision col){
		if(col.gameObject.CompareTag("boulder")){
			//col.gameObject.SetActive (false);
			Destroy(col.gameObject);
		}
	}
//	void OnTriggerEnter(Collider other){
//		if (other.gameObject.CompareTag ("boulder")) {
//			other.gameObject.SetActive (false);
////			Destroy(other.gameObject);
//		}
//	}
}
