using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinchController : MonoBehaviour {


	public int speed; 
	float screenWidthinWorldUnits;
	float screenoffset;
	// Use this for initialization
	void Start () {

		screenWidthinWorldUnits = Camera.main.aspect * Camera.main.orthographicSize;
		screenoffset = screenWidthinWorldUnits - ((transform.localScale.x / 2)+ 0.98f);
	}


	// Update is called once per frame
	void Update () {

		float direction = Input.GetAxisRaw ("Horizontal");
		float velocity = direction * speed;
		transform.Translate (Vector3.right * velocity * Time.deltaTime);


		if (transform.position.x < -screenoffset)
			transform.position = new Vector2(-screenoffset, transform.position.y);
		if(transform.position.x > screenoffset)
			transform.position = new Vector2(screenoffset, transform.position.y);
			


	}
}
