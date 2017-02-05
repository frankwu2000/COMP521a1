using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	public Maze MazeScript;

	// Use this for initialization
	void Start () {
		MazeScript.enabled = false;
		BeginGame ();
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.KeypadEnter)){
			RestartGame ();
		}
	}
	public void BeginGame(){
		if(Input.GetKeyDown(KeyCode.KeypadEnter)){
			MazeScript.enabled = true;
		}


	}

	public void RestartGame(){
		MazeScript.enabled = true;;
	}
}
