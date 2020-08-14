using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class NodeDatabase {
	public static List<GameObject> allNodes = new List<GameObject> ();
	static GameObject[] tempStorage;

	// Use this for initialization
	public static void init()
	{
		tempStorage = new GameObject[GameObject.FindGameObjectsWithTag ("Node").Length];
		tempStorage = GameObject.FindGameObjectsWithTag ("Node");

		for (int i = 0; i < tempStorage.Length; i++) 
		{
			allNodes.Add(tempStorage[i]);
		}

	
	}

}
