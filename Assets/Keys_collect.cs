using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Keys_collect : MonoBehaviour {


	public Maze maze_script;
	public int keys;
	public Text ScoreText;
	public Text VictoryText;


	// Use this for initialization
	void Start () {
		keys = 0;
		setScoreText ();
		VictoryText.text = "";

	}

	// Update is called once per frame
	void Update () {

		//restart the game
		if (Input.GetKeyDown (KeyCode.Q)) {
			//Destroy (ScoreText);

			Start ();
			gameObject.transform.position = new Vector3 (-37.66f,42f,-12f);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.CompareTag ("key")) {
			other.gameObject.SetActive (false);
			keys++;
			setScoreText ();
		}
		if (other.gameObject.CompareTag ("Finish")) {
			if (keys == 3) {
				//victory
				VictoryText.text = "Good Job You Win! Press Q to restart the game!";
			} else {
				//not enough keys
				ScoreText.text = "Not Enough Keys! " + keys + " keys now";

				VictoryText.text = "";

			}

		}
		if (other.gameObject.CompareTag ("boulder")) {
			VictoryText.text = "You lose!!! Press Q to restart.";
		}

		//at entrance of maze, boulder start moving
		if (other.gameObject.CompareTag ("maze_start_ring")) {
			maze_script.boulder_Start ();
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
