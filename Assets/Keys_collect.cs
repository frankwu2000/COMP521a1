using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keys_collect : MonoBehaviour {

	public int keys;
	public Text ScoreText;


	// Use this for initialization
	void Start () {
		keys = 0;
		setScoreText ();
	}
	
	// Update is called once per frame
	void Update () {
		//restart the game
		if (Input.GetKeyDown (KeyCode.Q)) {
			//Destroy (ScoreText);
			Start ();
			gameObject.transform.position = new Vector3 (-10,22,-12);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("key")) {
			other.gameObject.SetActive (false);
			keys++;
			setScoreText ();
		}
	}

	void setScoreText(){
		if (keys == 3) {
			ScoreText.text = "You got three keys!";
		} else {
			ScoreText.text = "Number of Keys: " + keys;
		}

	}
}
