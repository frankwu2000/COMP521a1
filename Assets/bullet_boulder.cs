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

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("boulder")) {
			other.gameObject.SetActive (false);
		}
	}
}
