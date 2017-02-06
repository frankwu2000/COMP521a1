using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class boulder_script : MonoBehaviour {
	
	public Text VictoryText;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("boulder")) {
			VictoryText.text = "You lose!!!";
		}
	}
}
