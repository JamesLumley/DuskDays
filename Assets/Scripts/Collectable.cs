using UnityEngine;
using System.Collections;

public class Collectable : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerEnter(Collider c)
	{
		//if player collides with it, collect item
		if(c.tag == "Player")
		{
			Score.collectables++;
			Score.allCollectables++;
			Destroy (gameObject);
		}
	}
}
