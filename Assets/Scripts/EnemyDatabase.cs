using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class EnemyDatabase{
	
	public static List<GameObject> enemies = new List<GameObject> ();
	static GameObject[] tempStorage;

	public static void init()
	{
		
		tempStorage = new GameObject[GameObject.FindGameObjectsWithTag ("Enemy").Length];
		tempStorage = GameObject.FindGameObjectsWithTag ("Enemy");
		
		
		for (int i = 0; i < tempStorage.Length; i++) 
		{		
			enemies.Add(tempStorage[i]);
		}
	}

	public static GameObject getSelected()
	{
		foreach (GameObject enemy in enemies) 
		{
			if(enemy.GetComponent<Enemy_Controller>().selected == true)
			{
				return enemy;
			}
			else
			{continue;}
		}
		return null;
	}

	public static void deleteList()
	{
		enemies.Clear ();
	}
}
