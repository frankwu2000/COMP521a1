using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour {

	public GameObject bullet;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			
			GameObject temp  = Instantiate (bullet,gameObject.transform.position,Quaternion.identity,gameObject.transform);
			temp.transform.position += new Vector3 (0f,0.5f,0f);
			Rigidbody rb = temp.GetComponent<Rigidbody> ();
			Camera main_cam = Camera.main;
			Vector3 force_dir = main_cam.transform.forward;
			force_dir.y += 0.3f;
			rb.AddForce (force_dir*100);
		}
	}
	
}
