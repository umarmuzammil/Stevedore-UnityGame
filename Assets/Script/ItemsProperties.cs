using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemsProperties : MonoBehaviour {

	//[Range(0,50)]
	//public int weight = 5;
	//Rigidbody2D thisRigidBody;



	public enum type{HingeBased, Parenting};
	public type physicsType;

	int attachCount = 0; 
	public delegate void ItemPicked(bool state);
	public event ItemPicked ItemEvent; 

	void Start()
	{

		
	}
	void Update()
	{
		
		if (Input.GetAxisRaw ("Jump") > 0.01)
			DropItem ();
	}


	void DropItem()
	{
			if ((physicsType == type.HingeBased) && (transform.GetComponent<HingeJoint2D> () != null)) {
				Destroy (transform.GetComponent<HingeJoint2D> ());

				if (ItemEvent != null)
					ItemEvent (false);
			} 

			else if ((physicsType == type.Parenting) && (transform.parent == GameObject.FindGameObjectWithTag ("magnet").transform)) {
				
				transform.parent = null;
				transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;

				attachCount--;
				transform.SetParent (GameObject.FindGameObjectWithTag ("container").transform);


				if (ItemEvent != null)
					ItemEvent (false);					
			}

			
		
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.tag == "magnet" && attachCount == 0) {

			if ((physicsType == type.HingeBased) &&  (transform.GetComponent<HingeJoint2D>() == null)) {

				Rigidbody2D connectedBody = other.transform.parent.transform.GetComponent<Rigidbody2D> ();
				transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Dynamic;

				HingeJoint2D joint = transform.gameObject.AddComponent<HingeJoint2D> ();
				JointAngleLimits2D limits;


				joint.autoConfigureConnectedAnchor = false;
				joint.autoConfigureConnectedAnchor = false;
				joint.useLimits = true;
				joint.connectedBody = connectedBody;

				//Anchor 
				Vector2 anchor = joint.anchor; 
				anchor.x = 0;
				anchor.y = 8f;
				joint.anchor = anchor;

				//ConnectedAnchor
				Vector2 connectedAnchor = joint.connectedAnchor;
				connectedAnchor.x = 0;
				connectedAnchor.y = 0f;		
				joint.connectedAnchor = connectedAnchor;

				limits = joint.limits;
				limits.max = -5;
				limits.min = 5f;
				joint.limits = limits;

				if (ItemEvent != null)
					ItemEvent (true);
						
			} 
			else if ((physicsType == type.Parenting) && (transform.parent == GameObject.FindGameObjectWithTag ("container").transform)) {
				transform.GetComponent<Rigidbody2D> ().bodyType = RigidbodyType2D.Kinematic;
				transform.SetParent (other.transform);

				attachCount++;

				if (ItemEvent != null)
					ItemEvent (true);
			}
		}
				
	}

}
