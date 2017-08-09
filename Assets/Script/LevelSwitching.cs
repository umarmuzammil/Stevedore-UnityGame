using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSwitching : MonoBehaviour {

	public void SceneLoadByName(string levelname)
	{
		SceneManager.LoadScene (levelname);
	}
}
