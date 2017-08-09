using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnItems : MonoBehaviour {

	public bool randomize;
	public GameObject[] items;
	public GameObject[] spawnLocations;
	public int spawnCount;
	public int maxDock = 2;

	public float itemsWeight = 3f; 
	public float itemsGravity =1.2f;


	private float itemHeight;
	private List<int> spawnPoints = new List<int>();


	void Start () {
		if (CheckItemsLimit () && randomize) {
		
			GenerateLocations ();
			SpawnWeights ();
		}
		SetWeight ();

	}


	void SetWeight()
	{
		foreach (Transform child in transform) {
			child.GetComponent<Rigidbody2D> ().mass = itemsWeight;
			child.GetComponent<Rigidbody2D> ().gravityScale = itemsGravity;
		}

	}

	bool CheckItemsLimit()
	{
		int maxitems = maxDock * items.Length;
		if (spawnCount > maxitems) {
			print ("Spawn Count exceeds the available locations");
			print ("Increase DockLimit or Increase spawnLocations or reduce spawnCount ");
			return false;
		} else
			return true;
		
	}
		
	public bool DockLimit(List<int> List, int val)
	{
		int limit = 0;
		int i = 0;

		while (i < List.Count) {
			
			if (List[i] == val) {
				limit++;
				}
			i++;
		}
		if (limit > maxDock) 
			return true;
		 else
			return false;

	}

	void GenerateLocations()
	{

		int i = 0;
		while (i < spawnCount) {
			int localposition = Random.Range (0, spawnLocations.Length);
			spawnPoints.Add(localposition);

			while (DockLimit (spawnPoints, localposition)) {
				localposition = Random.Range (0, spawnLocations.Length);
				spawnPoints [i] = localposition;				
			}

			i++;
		}

	}

	void SpawnWeights()
	{
		int currentIndex = 0;

		List<int> placedweights = new List<int> ();

		while (currentIndex < spawnCount) {
			int j = 0;
			int height = 0;

			while (j < spawnPoints.Count) {				
				if(spawnPoints[currentIndex] == spawnPoints[j] )
				{
					height++;
				}

				j++;
			}

			float yOffset = 0;

			if (placedweights.Contains (spawnPoints [currentIndex])) {
				if (height == 1)
					yOffset = 0;
				else if (height == 2)
					yOffset = 1.45f;
			} else
				placedweights.Add (spawnPoints [currentIndex]);

			int WeightIndex = Random.Range (0, items.Length);

			Vector2 Position = new Vector2(spawnLocations [spawnPoints [currentIndex]].transform.position.x, spawnLocations [spawnPoints [currentIndex]].transform.position.y + yOffset );
			GameObject obj = Instantiate (items[WeightIndex], Position , Quaternion.identity);
			obj.transform.SetParent (transform);


			currentIndex++;
		
		}
	}

}

