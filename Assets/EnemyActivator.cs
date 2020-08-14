using UnityEngine;
using System.Collections.Generic;

public class EnemyActivator : MonoBehaviour {
	public List<GameObject> toActivate = new List<GameObject>();
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter()
	{
		foreach(GameObject enemy in toActivate)
		{
			enemy.GetComponent<Enemy_Controller>().active = true;
		}
		Destroy (gameObject);
	}
}
