using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class RopeController : MonoBehaviour {

	[Range(0,12)]
	public int length = 2;
	[Range(0,14)]
	public int maxLength = 13; 
	[Range(0,5)]
	public int maxLengthWithDock = 5;
	[Range(0f,1f)]
	public float pullDelayInSecs = 0.4f;


	public GameObject chainPrefab;
	public GameObject Magnet;

	public Transform winch;

	private bool attached;
	private Rigidbody2D WinchRb;
	private List <GameObject> chainList = new List<GameObject>();
	private List<ItemsProperties> itemsList = null;

	void Start () {		

		//variables Initializations
		itemsList = new List<ItemsProperties>(FindObjectsOfType<ItemsProperties>());
		WinchRb = winch.GetComponent<Rigidbody2D> ();
		attached = false;

		InitChain (); 			//Initilize Chain & Magnet
		SpawnMagnet ();
		SubItemsEvents ();  		//Subscribe to Items Event
		StartCoroutine ("RopeDynamics"); 		//Rope Gameplay Physics

	}

	void SubItemsEvents()
	{
		foreach(ItemsProperties items in itemsList)
			items.ItemEvent += ItemPicked;		
	}

	void ItemPicked(bool state)
	{
		attached = state;
	}

	void SetHingeProperties(GameObject thisObject, Rigidbody2D thisRigidbody, [Optional]int index)
	{		
		HingeJoint2D Joint = thisObject.GetComponent<HingeJoint2D> ();
		JointAngleLimits2D limits;

		Joint.autoConfigureConnectedAnchor = false;
		Joint.useLimits = true;
		Joint.connectedBody = thisRigidbody;

		//AnchorsSetup

		if (thisRigidbody.transform == winch) {

			//Anchor 
			Vector2 anchor = Joint.anchor; 
			anchor.x = 0;
			anchor.y = 3f;
			Joint.anchor = anchor;

			//ConnectedAnchor
			Vector2 connectedAnchor = Joint.connectedAnchor;
			connectedAnchor.x = 0;
			connectedAnchor.y = 0f;		
			Joint.connectedAnchor = connectedAnchor;
		} 
		else {

			if (index > 0) {

				float tempY = chainList [index - 1].GetComponent<HingeJoint2D> ().anchor.y - 1f;
				//Anchor
				Vector2 anchor = Joint.anchor;
				anchor.x = 0;
				anchor.y = tempY;
				Joint.anchor = anchor;

				//ConnectedAnchor

				Vector2 connectedAnchor = Joint.connectedAnchor;
				connectedAnchor.x = 0;
				connectedAnchor.y = 0f;
				Joint.connectedAnchor = connectedAnchor;
			
			
			
			} else {
				//Anchor
				Vector2 anchor = Joint.anchor;
				anchor.x = 0;
				anchor.y += 1.8f;
				Joint.anchor = anchor;

				//ConnectedAnchor

				Vector2 connectedAnchor = Joint.connectedAnchor;
				connectedAnchor.x = 0;
				connectedAnchor.y = 0f;
				Joint.connectedAnchor = connectedAnchor;			
			}


		}
		//Set Limits
		limits = Joint.limits;
		limits.max = -10;
		limits.min = 10f;
		Joint.limits = limits;

		thisObject.transform.SetParent (transform);
	}


	IEnumerator RopeDynamics()
	{
		for (;;) {
		
			float verticalControl = Input.GetAxisRaw ("Vertical");

			if (verticalControl > 0.05 && chainList.Count < maxLength){			

				if (!attached) {
					GameObject chainPiece = Instantiate (chainPrefab, new Vector2 (winch.position.x, 4.2f), Quaternion.identity);
					Rigidbody2D thisRigidbody = chainPiece.GetComponent<Rigidbody2D> ();
					List<GameObject> tempList = new List<GameObject> ();


					tempList.Add (chainPiece);

					for (int i = 0; i < chainList.Count; i++) {
						tempList.Add (chainList [i]);
					}

					chainList.Clear ();
					chainList = tempList;

					SetHingeProperties (chainPiece, WinchRb);
					SetHingeProperties (chainList [1], thisRigidbody, 1);
				} else {

					if (chainList.Count < maxLengthWithDock) {

						GameObject chainPiece = Instantiate (chainPrefab, new Vector2 (winch.position.x, 4.2f), Quaternion.identity);
						Rigidbody2D thisRigidbody = chainPiece.GetComponent<Rigidbody2D> ();
						List<GameObject> tempList = new List<GameObject> ();


						tempList.Add (chainPiece);

						for (int i = 0; i < chainList.Count; i++) {
							tempList.Add (chainList [i]);
						}

						chainList.Clear ();
						chainList = tempList;

						SetHingeProperties (chainPiece, WinchRb);
						SetHingeProperties (chainList [1], thisRigidbody, 1);
					
					}
				}



			} else if (verticalControl < -0.05 && chainList.Count > 3) {

				List<GameObject> tempList = new List<GameObject> ();

				for (int i = 1; i < chainList.Count; i++) {
					tempList.Add (chainList [i]);
				}
				SetHingeProperties (tempList[0], WinchRb);
				Destroy (chainList [0]);
				chainList.Clear ();
				chainList = tempList;
			}

			if (verticalControl != 0)
				RenameRope ();

			float waitTime = pullDelayInSecs; 
			yield return new WaitForSeconds(waitTime);
		
		}
	}


	void SpawnMagnet()
	{
		GameObject magnet = Instantiate (Magnet, new Vector2 (winch.position.x, chainList[chainList.Count-1].transform.position.y - 0.7f), Quaternion.identity);
		magnet.transform.SetParent (chainList[chainList.Count-1].transform);
	}


	void InitChain()
	{		
		GameObject currentpiece = Instantiate (chainPrefab, new Vector2 (winch.position.x, -0.25f), Quaternion.identity); 	
		SetHingeProperties (currentpiece, WinchRb);
		chainList.Add (currentpiece);
		GameObject previouspiece = currentpiece;

		for (int i = 0; i < length; i++) {

			GameObject chain = Instantiate (chainPrefab, new Vector2 (winch.position.x, previouspiece.transform.position.y - 0.22f), Quaternion.identity);
			SetHingeProperties (chain, previouspiece.GetComponent<Rigidbody2D> ());
			chainList.Add (chain);
			previouspiece = chain;
		}

		RenameRope ();
	}

	void RenameRope()
	{
		for(int i=0; i<chainList.Count; i++)
		{			
			chainList[i].name = "chain" + i.ToString ();
		}	
	}


}
