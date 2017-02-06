using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Boulder_Move : MonoBehaviour {
	public Vector3[] boulder_position;
	public int count_move;

	// Use this for initialization
	void Start () {
		

	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.Z)){
			count_move++;
			gameObject.transform.position = boulder_position[count_move];
		}
	}
}
